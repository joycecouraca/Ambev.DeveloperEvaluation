using Ambev.DeveloperEvaluation.Domain.Common;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Upsert(TEntity entity);
    void Delete(TEntity entity);
    Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
