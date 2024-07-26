using search_product_mvc.Models;

namespace search_product_mvc.Services;

public interface ILuceneService
{
    public void Add(Product product);
    public void AddRange(IEnumerable<Product> products);
    public void Commit();
    public void Clear();
    public IEnumerable<Product> Search(string query, int maxHits = 10);
}