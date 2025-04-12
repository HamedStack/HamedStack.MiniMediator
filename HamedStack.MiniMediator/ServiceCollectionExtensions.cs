using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HamedStack.MiniMediator;

/// <summary>
/// Extension methods for registering MiniMediator with <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MiniMediator services and registers handlers from specified assemblies.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="assemblies">Assemblies to scan for handlers.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMiniMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddSingleton<IMediator, Mediator>();

        var allTypes = assemblies.SelectMany(a => a.GetTypes()).ToArray();

        foreach (var type in allTypes)
        {
            var interfaces = type.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType)
                {
                    var def = @interface.GetGenericTypeDefinition();
                    if (def == typeof(IRequestHandler<,>) ||
                        def == typeof(INotificationHandler<>) ||
                        def == typeof(IPipelineBehavior<,>))
                    {
                        services.AddTransient(@interface, type);
                    }
                }
            }
        }

        return services;
    }
}