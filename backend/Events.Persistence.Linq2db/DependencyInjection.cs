using Events.Application;
using Events.Domain.Repositories;
using Events.Persistence.Linq2db.Configuration;
using Events.Persistence.Linq2db.Repositories;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Persistence.Linq2db;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceLinq2db(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var fluentMapping = new FluentMappingBuilder();
        fluentMapping.BuildSchema();

        services.AddLinqToDBContext<DbContext>((provider, options) =>
            options
                .UsePostgreSQL(configuration.GetConnectionString("Postgres2db")!)
                .UseMappingSchema(fluentMapping.MappingSchema)
                .UseDefaultLogging(provider)
        );

        services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<DbContext>());

        services.AddScoped<IEventRepository, EventRepository>();

        return services;
    }
}