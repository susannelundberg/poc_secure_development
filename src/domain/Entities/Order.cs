namespace domain.Entities;

public class Order
{
    public int Id { get; set; }
    public Guid OrderNumber { get; set; }
    public required List<OrderItem> OrderItems { get; set; }
    public decimal TotalPrice { get; set; }
    public required string UserId { get; set; }
}
