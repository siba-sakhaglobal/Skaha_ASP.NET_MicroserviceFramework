namespace HorizonERP.Shared.EventContracts;

public abstract record IntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public string EventType => GetType().Name;
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public Guid TenantId { get; init; }
    public Guid CorrelationId { get; init; }
    public Guid UserId { get; init; }
}
