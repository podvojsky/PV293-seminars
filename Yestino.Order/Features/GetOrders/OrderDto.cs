using Yestino.Order.Entities;

namespace Yestino.Order.Features.GetOrders;

public class OrderDto
{
    public decimal TotalPrice { get; set; }

    public bool IsPayed { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}
