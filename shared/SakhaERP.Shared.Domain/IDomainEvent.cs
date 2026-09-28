namespace SakhaERP.Shared.Domain;
public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
