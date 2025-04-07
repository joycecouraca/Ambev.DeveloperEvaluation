using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class UnitOfWorkDbContext :  IUnitOfWork
{
    private readonly DefaultContext _context;
    public IRepository<Product> Products { get; }

    public IRepository<Sale> Sales { get; }

    public IRepository<User> Users { get; }

    public UnitOfWorkDbContext(DefaultContext context, IRepository<Product> productRepository, IRepository<Sale> salesRepository, IRepository<User> userRepository)
    {
        _context = context;
        Products = productRepository;
        Sales = salesRepository;
        Users = userRepository;
    }

    public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        _ = await _context.SaveChangesAsync(cancellationToken);
    }
}
