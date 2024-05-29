using Events.Application;
using Events.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<IEventRepository, EventRepository>();
        services.AddSingleton<IUnitOfWork, UnitOfWork>();

        return services;
    }
}