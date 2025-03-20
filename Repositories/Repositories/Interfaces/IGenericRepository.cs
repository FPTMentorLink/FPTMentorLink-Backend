using System.Linq.Expressions;
using Repositories.Entities.Base;

namespace Repositories.Repositories.Interfaces;

public interface IGenericRepository<TEntity, in TKey> where TEntity : class, IEntity<TKey>
{
    IQueryable<TEntity> GetQueryable();

    /// <summary>
    /// As tracking
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// As no tracking
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<TEntity?> FindByIdAsNoTrackingAsync(TKey id, CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// As tracking
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// As tracking
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Check if any entity exists and return bool type for optimization
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Get a queryable with as no tracking
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includes);

    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
}

// For convenience when using Guid as key
public interface IGenericRepository<TEntity> : IGenericRepository<TEntity, Guid> where TEntity : class, IEntity<Guid>;