using System.ComponentModel.DataAnnotations;

namespace application.DTOs.Products;

public class UpdateProductDTO
{
    [StringLength(50, MinimumLength = 1)]
    public required string Name { get; set; }
    [Range(1, 1000000)]
    public required decimal Price { get; set; }
}