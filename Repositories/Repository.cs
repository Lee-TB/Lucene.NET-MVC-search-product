
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using search_product_mvc.Data;

namespace search_product_mvc.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
  private readonly AppDbContext _context;
  private readonly DbSet<T> _dbSet;

  public Repository(AppDbContext context)
  {
    _context = context;
    _dbSet = context.Set<T>();
  }

  public void Create(T entity)
  {
    _context.Add(entity);
  }

  public void Update(T entity)
  {
    _context.Update(entity);
  }

  public void Delete(int id)
  {
    var entity = _dbSet.Find(id);
    if (entity != null)
    {
      _context.Remove(entity);
    }
  }

  public void Delete(T entity)
  {
    _context.Remove(entity);
  }

  public async Task<IEnumerable<T>> GetAllAsync()
  {
    return await _dbSet.ToListAsync();
  }

  public async Task<T?> GetByIdAsync(int id)
  {
    return await _dbSet.FindAsync(id);
  }

  public Task<T?> GetByIdAsync(Guid id)
  {
    var member = Expression.Parameter(typeof(T), "e");
    var property = Expression.Property(member, "Id");
    var constant = Expression.Constant(id);
    var equal = Expression.Equal(property, constant);
    var lambda = Expression.Lambda<Func<T, bool>>(equal, member);

    return _dbSet.FirstOrDefaultAsync(lambda);
  }

  public async Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> condition)
  {
    return await _dbSet.Where(condition).ToListAsync();
  }

  public async Task<int> CountAsync()
  {
    return await _dbSet.CountAsync();
  }

  public async Task SaveAsync()
  {
    await _context.SaveChangesAsync();
  }


}