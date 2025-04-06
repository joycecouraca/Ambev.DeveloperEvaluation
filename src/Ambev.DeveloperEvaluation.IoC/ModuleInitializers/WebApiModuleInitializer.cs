using Ambev.DeveloperEvaluation.IoC.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers
{
    public class WebApiModuleInitializer : IModuleInitializer
    {
        public void Initialize(WebApplicationBuilder builder)
        {
            var applicationAssembly = Assembly.Load(new AssemblyName("Ambev.DeveloperEvaluation.WebApi"));
            builder.Services.AddValidatorsFromAssembly(applicationAssembly);

            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();
        }
    }
}
