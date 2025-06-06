using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Restia.Common.Common.Interfaces;
using System;
using System.Linq;

namespace Restia.Common.Common
{
    public static class Startup
    {
        /// <summary>
        /// Automatically registers all services implementing the <see cref="ITransientService"/> interface
        /// with a Transient lifetime, and all services implementing the <see cref="IScopedService"/> interface
        /// with a Scoped lifetime.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the services to.</param>
        /// <returns>The IServiceCollection with the registered services.</returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // If you registered the same service and its implement multiple lifetime, you resolve the service with the last lifetime you registered (scoped)
            // But both of these registration is registered by design, which you can resolve by IEnumerable<Dependency> dependencies (only when using Obsolete method)
            return services
                .AddServices(typeof(ITransientService), ServiceLifetime.Transient)
                .AddServices(typeof(IScopedService), ServiceLifetime.Scoped);
        }

        private static IServiceCollection AddServices(
            this IServiceCollection services,
            Type interfaceType,
            ServiceLifetime lifetime)
        {
            var interfaceTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => interfaceType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                .Select(type =>
                    new
                    {
                        Service = type.GetInterfaces().FirstOrDefault(),
                        Implementation = type
                    })
                .Where(type => type.Service != null && interfaceType.IsAssignableFrom(type.Service));

            foreach (var type in interfaceTypes)
            {
                services.TryAddService(type.Service, type.Implementation, lifetime);
            }

            return services;
        }

        private static IServiceCollection TryAddService(
            this IServiceCollection services,
            Type serviceType,
            Type implementationType,
            ServiceLifetime lifetime)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);

            services.TryAdd(descriptor);
            return services;
        }

        [Obsolete]
        private static IServiceCollection AddService(
            this IServiceCollection services,
            Type serviceType,
            Type implementationType,
            ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    return services.AddTransient(serviceType, implementationType);
                case ServiceLifetime.Scoped:
                    return services.AddScoped(serviceType, implementationType);
                case ServiceLifetime.Singleton:
                    return services.AddSingleton(serviceType, implementationType);
                default:
                    throw new ArgumentException("Invalid lifeTime", nameof(lifetime));
            }
        }
    }
}
