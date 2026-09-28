namespace SakhaERP.Shared.Domain;
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
