using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.Order.Infrastructure;
using Yestino.OrderContracts.DomainEvents;

namespace Yestino.Order.Features.ChangeOrderItemQuantity;

public record ChangeItemQuantityCommand(Guid ProductId, int NewQuantity);

public class ChangeOrderItemQuantityEndpoint
{
    [WolverinePut("/orders/{orderId}/change-item-quantity")]
    public static (IResult, IStorageAction<Entities.Order>, OrderItemQuantityChanged?) ChangeItemQuantity(
        Guid orderId, ChangeItemQuantityCommand command, OrderDbContext db, CancellationToken cancellationToken)
    {
        var order = db.Orders.Include(order => order.Items).FirstOrDefault(order => order.Id == orderId);
        if (order == null)
            return (
                Results.BadRequest("Order was not found"),
                Storage.Nothing<Entities.Order>(),
                null
            );

        if (command.NewQuantity < 0)
            return (
                Results.BadRequest("Quantity cannot be negative"),
                Storage.Nothing<Entities.Order>(),
                null
            );

        var orderItem = order.Items.FirstOrDefault(orderItem => orderItem.ProductId == command.ProductId);
        var product = db.ProductReadModels.FirstOrDefault(product => product.Id == command.ProductId);

        if (orderItem == null)
        {
            if (command.NewQuantity > 0)
            {
                if (product == null)
                    return (
                        Results.BadRequest("Product does not exist"),
                        Storage.Nothing<Entities.Order>(),
                        null
                    );

                // ADD NEW ITEM
                var newItem = order.AddItem(product, command.NewQuantity);

                // Return update + correct event
                return (
                    Results.NoContent(),
                    Storage.Update(order),
                    new OrderItemQuantityChanged(order.Id, newItem.Id, product.Id, newItem.Quantity)
                );
            }
        }
        else
        {
            if (orderItem.Quantity == command.NewQuantity)
                return (Results.NoContent(), Storage.Nothing<Entities.Order>(), null);

            if (command.NewQuantity == 0)
                order.RemoveItem(command.ProductId);
            else
                order.ChangeItemQuantity(orderItem.Id, command.NewQuantity);
        }

        return (Results.NoContent(), Storage.Update(order),
            new OrderItemQuantityChanged(order.Id, orderItem?.Id, product?.Id, command.NewQuantity));
    }
}
