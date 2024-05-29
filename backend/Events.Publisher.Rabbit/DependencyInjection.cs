using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Events.Publisher.Rabbit;

public static class DependencyInjection
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPublisher>(new RabbitPublisher(configuration.GetConnectionString("RabbitMQ")!));

        return services;
    }
}