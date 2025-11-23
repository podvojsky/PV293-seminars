using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Yestino.Order.Infrastructure;

namespace Yestino.Order.Features.GetOrders;

public class GetOrdersEndpoint
{
    [WolverineGet("/orders")]
    public static async Task<ICollection<OrderDto>> GetOrders([FromQuery] bool onlyPaid,
        OrderDbContext dbContext, CancellationToken cancellationToken)
    {
        var query = dbContext.Orders.AsQueryable();

        if (onlyPaid) query = query.Where(order => order.IsPayed);

        return await query
            .Select(order => new OrderDto
            {
                TotalPrice = order.TotalPrice,
                IsPayed = order.IsPayed,
                Items = order.Items
            })
            .ToListAsync(cancellationToken);
    }
}
