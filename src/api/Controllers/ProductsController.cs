using application.DTOs.Products;
using domain.Entities;
using infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[Authorize]
[ApiController]
[Route("api/products")]
public class ProductsController(AppDbContext context) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<ProductDTO>>> GetProducts([FromQuery] string? search)
    {
        try
        {
            var query = context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            var products = await query
                .Select(p => new ProductDTO { Id = p.Id, Name = p.Name, Price = p.Price, ImageUrl = p.ImageUrl })
                .ToListAsync();

            return Ok(products);
        }
        catch
        {
            return StatusCode(500, "Något gick fel");
        }
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProduct(int id)
    {
        try
        {
            var product = await context.Products.FindAsync(id);

            if (product is null) return NotFound();

            return Ok(new ProductDTO { Id = product.Id, Name = product.Name, Price = product.Price, ImageUrl = product.ImageUrl });
        }
        catch
        {
            return StatusCode(500, "Något gick fel");
        }

    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDTO>> CreateProduct(CreateProductDTO model)
    {
        try
        {
            var product = new Product { Name = model.Name, Price = model.Price };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            var dto = new ProductDTO { Id = product.Id, Name = product.Name, Price = product.Price, ImageUrl = product.ImageUrl };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, dto);
        }
        catch
        {
            return StatusCode(500, "Något gick fel");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, UpdateProductDTO model)
    {
        try
        {
            var product = await context.Products.FindAsync(id);

            if (product is null) return NotFound();

            product.Name = model.Name;
            product.Price = model.Price;
            await context.SaveChangesAsync();

            return NoContent();
        }
        catch
        {
            return StatusCode(500, "Något gick fel");
        }
    }
}