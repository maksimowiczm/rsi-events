using Events.Application.Abstractions;
using Events.Domain.Repositories;
using Events.Persistence.Psql.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Persistence.Psql;

public static class DependencyInjection
{
    [Obsolete("EF 💀💀💀💀💀💀💀💀💀💀💀")]
    public static IServiceCollection AddPersistencePsql(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IEventRepository, EventRepository>();
        services
            .AddDbContext<EventsDbContext>(o =>
                o.UseNpgsql(configuration.GetConnectionString("Postgres"))
            );
        services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<EventsDbContext>());

        return services;
    }
}