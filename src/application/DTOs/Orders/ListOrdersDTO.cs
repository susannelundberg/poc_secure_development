namespace application.DTOs.Orders;

public class ListOrdersDTO
{
    public Guid OrderNumber { get; set; }
    public decimal TotalPrice { get; set; }
}
