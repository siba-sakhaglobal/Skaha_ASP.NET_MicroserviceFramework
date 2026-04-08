namespace HorizonERP.Shared.Domain;
public abstract class AuditableEntity : BaseEntity
{
    public Guid TenantId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid CreatedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
