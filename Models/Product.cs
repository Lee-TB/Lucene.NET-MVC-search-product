using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace search_product_mvc.Models;
public class Product
{
  [DatabaseGenerated(DatabaseGeneratedOption.None)]
  public Guid Id { get; set; }
  [Required]
  [StringLength(100)]
  public string Title { get; set; }
  [StringLength(500)]
  public string? Description { get; set; }
  [DataType(DataType.Currency)]
  public decimal Price { get; set; }
}