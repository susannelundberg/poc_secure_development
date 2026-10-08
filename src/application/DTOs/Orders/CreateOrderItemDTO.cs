using System.ComponentModel.DataAnnotations;

namespace application.DTOs.Orders;

public class CreateOrderItemDTO
{
    public int ProductId { get; set; }
    
    [Range(1, 100)]
    public int Quantity { get; set; }
}
