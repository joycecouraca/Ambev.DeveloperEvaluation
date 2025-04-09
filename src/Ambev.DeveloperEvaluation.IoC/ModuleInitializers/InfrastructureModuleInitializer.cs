using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

[ExcludeFromCodeCoverage]
public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(DbContextRepository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWorkDbContext>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();
    }
}