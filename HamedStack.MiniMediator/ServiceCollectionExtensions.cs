using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HamedStack.MiniMediator;

/// <summary>
/// Extension methods for registering mediator services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds mediator services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="assemblies">The assemblies to scan for handler implementations. If not provided, all non-system assemblies in the current AppDomain will be scanned.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMiniMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddScoped<IMediator, Mediator>();
        if (assemblies.Length == 0)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a is { IsDynamic: false, FullName: not null } && !a.FullName.StartsWith("System") && !a.FullName.StartsWith("Microsoft"))
                .ToArray();
        }
        foreach (var assembly in assemblies)
        {
            try
            {
                var requestHandlers = assembly.GetTypes()
                    .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                                t.GetInterfaces().Any(i =>
                                    i.IsGenericType &&
                                    (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                                     i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))));
                foreach (var handler in requestHandlers)
                {
                    foreach (var handlerInterface in handler.GetInterfaces()
                                 .Where(i => i.IsGenericType &&
                                             (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                                              i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))))
                    {
                        services.AddTransient(handlerInterface, handler);
                    }
                }
                var notificationHandlers = assembly.GetTypes()
                    .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                                t.GetInterfaces().Any(i =>
                                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)));
                foreach (var handler in notificationHandlers)
                {
                    foreach (var handlerInterface in handler.GetInterfaces()
                                 .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)))
                    {
                        services.AddTransient(handlerInterface, handler);
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
                // Swallow exceptions from assemblies that cannot be loaded
            }
        }
        return services;
    }
}