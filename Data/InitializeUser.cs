using System.Text.Json;
using search_product_mvc.Models;

namespace search_product_mvc.Data;

public class InitializeUser
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Users.Any())
        {
            return;
        }

        var userFileText = File.ReadAllText("Data/users.json");
        var users = JsonSerializer.Deserialize<List<User>>(userFileText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var usersWithoutId = users.Select(u => new User
        {
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email
        }).ToList();
        context.Users.AddRange(usersWithoutId);
        context.SaveChanges();
        System.Console.WriteLine("Users added to database");
    }
}