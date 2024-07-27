using System.Linq.Expressions;

namespace search_product_mvc.Repositories;

public interface IRepository<T>
{
  Task<IEnumerable<T>> GetAllAsync();
  Task<T?> GetByIdAsync(int id);
  Task<T?> GetByIdAsync(Guid id);
  void Create(T entity);
  void Update(T entity);
  void Delete(int id);
  void Delete(T entity);
  Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> condition);
  Task<int> CountAsync();
  Task SaveAsync();
}