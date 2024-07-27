namespace search_product_mvc.Services;

public interface ILuceneService<TEntity>
{
    public void Add(TEntity entity);
    public void AddRange(IEnumerable<TEntity> entities);
    public void Update(TEntity entity);
    public void Delete(TEntity entity);
    public void Delete(int id);
    public void Commit();
    public void Clear();
    public IEnumerable<TEntity> Search(string query, int maxHits = 10);
}