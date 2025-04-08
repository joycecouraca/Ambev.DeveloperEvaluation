using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

[ExcludeFromCodeCoverage]
public class DbContextRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    public DbSet<TEntity> Entities { get; }

    public DbContextRepository(DefaultContext context)
    {
        Entities = context.Set<TEntity>();
    }

    public virtual void Add(TEntity entity)
    {
        Entities.Add(entity);
    }

    public virtual void Update(TEntity entity)
    {
        Entities.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        Entities.Remove(entity);
    }

    public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Entities.AnyAsync(predicate, cancellationToken);
    }

    public virtual Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Entities.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Entities.FirstAsync(x => x.Id == id, cancellationToken);
    }


    public virtual Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Entities.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<PaginatedList<TEntity>> GetPaginatedAsync(
        int page,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = Entities.AsNoTracking();

        if (filter is not null)
            query = query.Where(filter);

        if (orderBy is not null)
            query = orderBy(query);

        return await PaginatedList<TEntity>.CreateAsync(query, page, pageSize, cancellationToken);
    }
}
