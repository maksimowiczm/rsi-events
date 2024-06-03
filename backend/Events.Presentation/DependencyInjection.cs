using System.Reflection;
using Events.Application.Dto;
using Microsoft.Extensions.DependencyInjection;
using RiskFirst.Hateoas;
using RiskFirst.Hateoas.Polyfills;

namespace Events.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        // override IAssemblyLoader because riskfirst.hateoas takes EventDto assembly
        // https://github.com/riskfirst/riskfirst.hateoas/blob/fe5bb78b3e8b17dc69ca59081dcc04a47e21cc40/src/RiskFirst.Hateoas/LinksServicesCollectionExtensions.cs#L17
        services.AddSingleton<IAssemblyLoader, MyLoader>();

        services.AddLinks(c =>
            c.AddPolicy<EventDto>(p
                => p.RequireRoutedLink("get", "GetEventByIdRoute", e => new { e.Id })
            )
        );

        return services;
    }
}

internal class MyLoader : IAssemblyLoader
{
    public IEnumerable<Assembly> GetAssemblies()
    {
        yield return typeof(DependencyInjection).Assembly;
    }
}