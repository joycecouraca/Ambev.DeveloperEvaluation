using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IUnitOfWork
{
    IRepository<Product> Products { get; }
    Task CommitChangesAsync(CancellationToken cancellationToken = default);
}
