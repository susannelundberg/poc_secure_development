using System.ComponentModel.DataAnnotations;

namespace application.DTOs.Products;

public class ProductDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}