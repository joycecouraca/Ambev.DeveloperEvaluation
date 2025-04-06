using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IUnitOfWork
{
    IRepository<Product> Products { get; }
    IRepository<Sales> Sales { get; }
    IRepository<User> Users { get; }
    Task CommitChangesAsync(CancellationToken cancellationToken = default);
}
