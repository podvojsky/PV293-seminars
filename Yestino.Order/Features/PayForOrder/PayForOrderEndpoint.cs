using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.OrderContracts.DomainEvents;

namespace Yestino.Order.Features.PayForOrder;

public record PayForOrderCommand;

public class PayForOrderEndpoint
{
    [WolverinePut("/orders/{orderId}/pay")]
    public static (IResult, IStorageAction<Entities.Order>, OrderPayed?) PayForOrder([Entity] Entities.Order order,
        PayForOrderCommand command)
    {
        if (order.IsPayed)
            return (
                Results.BadRequest("Order was already payed for"),
                Storage.Nothing<Entities.Order>(),
                null
            );

        order.IsPayed = true;

        return (Results.NoContent(), Storage.Update(order), new OrderPayed(order.Id));
    }
}
