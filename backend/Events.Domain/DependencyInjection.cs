using Events.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<EventFactory>();

        return services;
    }
}