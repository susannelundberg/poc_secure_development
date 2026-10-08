using System.ComponentModel.DataAnnotations;

namespace application.DTOs.Orders;

public class CreateOrderDTO
{
    [MinLength(1)]
    public required List<CreateOrderItemDTO> OrderItems { get; set; }
}
