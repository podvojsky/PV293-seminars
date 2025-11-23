using Yestino.Common.Domain;

namespace Yestino.OrderContracts.DomainEvents;

public record OrderCreated(Guid AggregateId) : DomainEvent(AggregateId);
