using Yestino.Common.Domain;

namespace Yestino.OrderContracts.DomainEvents;

public record OrderPayed(Guid AggregateId) : DomainEvent(AggregateId);
