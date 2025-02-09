using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities.Base;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories;

public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return DbSet.AsQueryable();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? expression)
    {
        var query = DbSet.AsQueryable();
        if (expression != null)
        {
            query = query.Where(expression);
        }
        return await query.ToListAsync();
    }
    
    public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? expression)
    {
        var query = DbSet.AsQueryable();
        if (expression != null)
        {
            query = query.Where(expression);
        }

        return await query.SingleOrDefaultAsync();
    }
    
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? expression)
    {
        var query = DbSet.AsQueryable();
        if (expression != null)
        {
            query = query.Where(expression);
        }

        return await query.AnyAsync();
    }

    public virtual void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}

// For convenience when using Guid as key
public class GenericRepository<TEntity>(ApplicationDbContext context)
    : GenericRepository<TEntity, Guid>(context), IGenericRepository<TEntity>
    where TEntity : class, IEntity<Guid>;