using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class UnitOfWorkDbContext :  IUnitOfWork
{
    private readonly DefaultContext _context;
    public IRepository<Product> Products { get; }

    public ISaleRepository Sales { get; }

    public IRepository<User> Users { get; }
    public ILogger<UnitOfWorkDbContext> _logger { get; }

    public UnitOfWorkDbContext(DefaultContext context, IRepository<Product> productRepository, ISaleRepository salesRepository, IRepository<User> userRepository, ILogger<UnitOfWorkDbContext> logger)
    {
        _context = context;
        Products = productRepository;
        Sales = salesRepository;
        Users = userRepository;
        _logger = logger;
    }

    public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = _context.ChangeTracker.Entries<BaseEntity>()
                                      .Where(e => e.Entity is Sale && e.Entity.Events.Any())
                                      .ToList();

        var events = domainEntities.SelectMany(e => e.Entity.Events).ToList();
        
        _ = await _context.SaveChangesAsync(cancellationToken);

        foreach (var @event in events)
        {
            _logger.LogInformation($"Publish Domain Event: {@event.GetType().Name} - {@event}");            
        }
    }
}
