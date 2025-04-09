namespace Ambev.DeveloperEvaluation.Integration.Fixture;

using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;


public class IntegrationTestFixture
{
    public HttpClient Client { get; private set; } = default!;
    public IServiceProvider Services { get; private set; } = default!;

    public IntegrationTestFixture()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove o DbContext de produção
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    // Adiciona InMemory
                    services.AddDbContext<DefaultContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });

                    // Monta o provider e roda o seed
                    var provider = services.BuildServiceProvider();
                    provider.EnsureSeeded();

                    Services = provider;
                });
            });

        Client = application.CreateClient();
    }
}
