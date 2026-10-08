using System.Text.Json;
using domain.Entities;

namespace infrastructure.Data;

public class SeedData
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task SeedProducts(AppDbContext context)
    {
        if (context.Products.Any()) return;

        var path = Path.Combine(AppContext.BaseDirectory, "Data", "Seed", "data.json");
        var json = File.ReadAllText(path);
        var products = JsonSerializer.Deserialize<List<Product>>(json, options);

        if (products is not null && products.Count > 0)
        {
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}