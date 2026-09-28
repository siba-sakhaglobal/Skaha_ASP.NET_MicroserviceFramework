namespace SakhaERP.Shared.Security;

using Microsoft.AspNetCore.Http;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Tenant can come from JWT claim or X-Tenant-Id header
        if (!context.User.HasClaim(c => c.Type == "tenant_id"))
        {
            if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader)
                && Guid.TryParse(tenantHeader.FirstOrDefault(), out _))
            {
                // In production, validate tenant exists and is active
            }
        }

        await _next(context);
    }
}
