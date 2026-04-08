namespace HorizonERP.Shared.Security;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

public interface ITenantContext
{
    Guid TenantId { get; }
    Guid UserId { get; }
    string? UserRole { get; }
}

public class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId => GetClaimAsGuid("tenant_id");
    public Guid UserId => GetClaimAsGuid(ClaimTypes.NameIdentifier);
    public string? UserRole => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

    private Guid GetClaimAsGuid(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
        return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
    }
}
