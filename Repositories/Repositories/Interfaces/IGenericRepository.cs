using System.Linq.Expressions;
using Repositories.Entities.Base;

namespace Repositories.Repositories.Interfaces;

public interface IGenericRepository<TEntity, in TKey> where TEntity : class, IEntity<TKey>
{
    IQueryable<TEntity> GetQueryable();
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? expression);
    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? expression);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? expression);

    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}

// For convenience when using Guid as key
public interface IGenericRepository<TEntity> : IGenericRepository<TEntity, Guid> where TEntity : class, IEntity<Guid>;