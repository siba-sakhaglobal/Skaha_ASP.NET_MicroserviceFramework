namespace SakhaERP.Shared.Security.MultiTenancy;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

/// <summary>
/// Middleware that resolves the current tenant for every request.
///
/// Flow:
/// 1. Extract tenant_id from JWT claim or X-Tenant-Id header
/// 2. Call ITenantConnectionResolver to get TenantInfo (from cache or Tenant Service)
/// 3. Store TenantInfo in HttpContext.Items for downstream consumption
/// 4. Reject request if tenant is not found or inactive (403)
///
/// The DbContext factory reads TenantInfo from HttpContext.Items to:
/// - Select the correct connection string (shared vs dedicated)
/// - Set tenant_id for global query filters (shared DB)
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public const string TenantInfoKey = "CurrentTenant";

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantConnectionResolver resolver)
    {
        // Skip tenant resolution for health checks and swagger
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (path.StartsWith("/health") || path.StartsWith("/swagger") || path.StartsWith("/favicon"))
        {
            await _next(context);
            return;
        }

        // 1. Extract tenant ID
        var tenantIdStr = context.User?.FindFirst("tenant_id")?.Value
            ?? context.Request.Headers["X-Tenant-Id"].FirstOrDefault();

        if (string.IsNullOrEmpty(tenantIdStr) || !Guid.TryParse(tenantIdStr, out var tenantId))
        {
            _logger.LogWarning("Request without valid tenant_id. Path: {Path}", path);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                errors = new[] { new { type = "TenantRequired", title = "Forbidden",
                    status = 403, detail = "A valid tenant_id is required (via JWT claim or X-Tenant-Id header)." } }
            });
            return;
        }

        // 2. Resolve tenant config
        var tenantInfo = await resolver.ResolveAsync(tenantId, context.RequestAborted);

        if (tenantInfo == null || !tenantInfo.IsActive)
        {
            _logger.LogWarning("Tenant {TenantId} not found or inactive", tenantId);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                errors = new[] { new { type = "TenantInactive", title = "Forbidden",
                    status = 403, detail = tenantInfo == null
                        ? "Tenant not found."
                        : "Tenant subscription has expired. Please renew." } }
            });
            return;
        }

        // 3. Store in HttpContext for DbContext factory and other components
        context.Items[TenantInfoKey] = tenantInfo;

        _logger.LogDebug("Tenant resolved: {TenantId} ({TenantName}, {DbType} DB)",
            tenantInfo.TenantId, tenantInfo.TenantName, tenantInfo.DbType);

        await _next(context);
    }
}
