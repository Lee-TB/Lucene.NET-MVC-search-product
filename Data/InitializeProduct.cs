using System.Text.Json;
using System.Text.Json.Serialization;
using search_product_mvc.Models;

namespace search_product_mvc.Data;

class ProductExtend : Product
{
    public new string Id { get; set; }
}

public class InitializeProduct
{
    private readonly AppDbContext _context;
    public InitializeProduct(AppDbContext context)
    {
        _context = context;
    }

    public void InitializeFromJson()
    {
        var productFileText = File.ReadAllText("Data/products.json");
        var products = JsonSerializer.Deserialize<List<ProductExtend>>(productFileText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        List<Product> products1 = products.Select(p => new Product
        {
            Id = Guid.NewGuid(),
            Title = p.Title,
            Description = p.Description,
            Price = p.Price
        }).ToList();
        _context.Products.AddRange(products1);
        _context.SaveChanges();
    }
}