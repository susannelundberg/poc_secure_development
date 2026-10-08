namespace domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public required Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal Sum { get; set; }
}
