using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.IoC.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<AssemblyScanner.AssemblyScanResult, bool>? filter = null, bool includeInternalTypes = false)
    {
        AssemblyScanner
            .FindValidatorsInAssembly(assembly, includeInternalTypes)
            .ForEach(scanResult => services.AddScanResult(scanResult, lifetime, filter));

        return services;
    }

    private static IServiceCollection AddScanResult(this IServiceCollection services, AssemblyScanner.AssemblyScanResult scanResult, ServiceLifetime lifetime, Func<AssemblyScanner.AssemblyScanResult, bool>? filter)
    {
        bool shouldRegister = filter?.Invoke(scanResult) ?? true;
        if (shouldRegister)
        {
            //Register as interface
            services.Add(
                new ServiceDescriptor(
                    serviceType: scanResult.InterfaceType,
                    implementationType: scanResult.ValidatorType,
                    lifetime: lifetime));

            //Register as self
            services.Add(
                new ServiceDescriptor(
                    serviceType: scanResult.ValidatorType,
                    implementationType: scanResult.ValidatorType,
                    lifetime: lifetime));
        }

        return services;
    }
}
