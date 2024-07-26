using Microsoft.EntityFrameworkCore;
using search_product_mvc.Data;
using search_product_mvc.Services;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("AppDbContext");
        options.UseSqlServer(connectionString);
    });
    builder.Services.AddSingleton(typeof(ILuceneService<>), typeof(LuceneService<>));
}

var app = builder.Build();
{
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}


