using Yestino.Common.Domain;

namespace Yestino.OrderContracts.DomainEvents;

public record OrderItemQuantityChanged(Guid AggregateId, Guid? ProductId, Guid? OrderItemId, int? NewQuantity)
    : DomainEvent(AggregateId);
