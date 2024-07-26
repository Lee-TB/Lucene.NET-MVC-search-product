using Microsoft.EntityFrameworkCore;
using search_product_mvc.Models;

namespace search_product_mvc.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}