using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class UnitOfWorkDbContext :  IUnitOfWork
{
    private readonly DefaultContext _context;
    public IRepository<Product> Products { get; }

    public UnitOfWorkDbContext(DefaultContext context, IRepository<Product> productRepository)
    {
        _context = context;
        Products = productRepository;
    }

    public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        _ = await _context.SaveChangesAsync(cancellationToken);
    }
}
