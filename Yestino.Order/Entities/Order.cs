using Yestino.Common.Domain;

namespace Yestino.Order.Entities;

public class Order : AggregateRoot
{
    public decimal TotalPrice { get; set; }

    public bool IsPayed { get; set; }

    public List<OrderItem> Items { get; set; } = [];

    public OrderItem AddItem(ProductReadModel product, int quantity)
    {
        var item = new OrderItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            UnitPrice = product.Price,
            Quantity = quantity
        };

        Items.Add(item);
        TotalPrice += product.Price * quantity;

        return item;
    }


    public OrderItem? RemoveItem(Guid productId)
    {
        var item = Items.SingleOrDefault(item => item.ProductId == productId);
        if (item == null) return null;

        TotalPrice -= item.UnitPrice * item.Quantity;
        Items.Remove(item);

        return item;
    }


    public OrderItem? ChangeItemQuantity(Guid orderItemId, int newQuantity)
    {
        var orderItem = Items.SingleOrDefault(item => item.Id == orderItemId);
        if (orderItem == null) return null;

        if (orderItem.Quantity > newQuantity)
            TotalPrice -= orderItem.UnitPrice * (orderItem.Quantity - newQuantity);
        else
            TotalPrice += orderItem.UnitPrice * (newQuantity - orderItem.Quantity);

        orderItem.ChangeQuantity(newQuantity);
        return orderItem;
    }
}
