using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.Order.Entities;
using Yestino.Order.Infrastructure;
using Yestino.OrderContracts.DomainEvents;

namespace Yestino.Order.Features.CreateOrder;

public record CreateOrderCommand
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}

public class CreateOrderEndpoint
{
    public static ProductReadModel? Before(CreateOrderCommand command, OrderDbContext dbContext)
    {
        return dbContext.ProductReadModels.FirstOrDefault(product => product.Id == command.ProductId);
    }

    [WolverinePost("/orders")]
    public static (IResult, IStorageAction<Entities.Order>, OrderCreated?) CreateOrder(
        [NotBody]
        ProductReadModel product,
        CreateOrderCommand command)
    {
        if (command.Quantity < 0)
            return (
                Results.BadRequest("Quantity cannot be negative"),
                Storage.Nothing<Entities.Order>(),
                null
            );

        var order = new Entities.Order
        {
            Id = Guid.NewGuid(),
            TotalPrice = 0,
            IsPayed = false
        };

        order.AddItem(product, command.Quantity);

        return (
            Results.Ok(order.Id),
            Storage.Insert(order),
            new OrderCreated(order.Id)
        );
    }
}
