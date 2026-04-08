namespace HorizonERP.Shared.Domain;
public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
