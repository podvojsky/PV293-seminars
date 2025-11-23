namespace Yestino.Order.Entities;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public void ChangeQuantity(int newQuantity)
    {
        Quantity = newQuantity;
    }
}
