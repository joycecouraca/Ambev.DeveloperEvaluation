using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Integration.Fixture;

public class MockedWebApplicationFactory : CustomWebApplicationFactory
{
    private readonly Action<IServiceCollection> _overrideServices;

    public MockedWebApplicationFactory(Action<IServiceCollection> overrideServices)
    {
        _overrideServices = overrideServices;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            _overrideServices(services);
        });
    }
}
