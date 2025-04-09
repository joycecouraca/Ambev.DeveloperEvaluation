using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Integration.Fixture;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public static IServiceProvider StaticServices { get; private set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            // Remove o DbContext original (provavelmente do PostgreSQL)
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Substitui pelo InMemory
            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Garante que o banco foi criado e populado
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            db.Database.EnsureCreated();

            // Seed de dados aqui se necessário            
            sp.EnsureSeeded();
            StaticServices = sp;
        });
    }
}