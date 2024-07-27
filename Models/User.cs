using System.ComponentModel.DataAnnotations;

namespace search_product_mvc.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }
    [Required]
    [StringLength(100)]
    public string LastName { get; set; }
    [Required]
    [StringLength(100)]
    public string Email { get; set; }
}