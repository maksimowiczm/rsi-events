using Events.Application;
using Events.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Events.Pdf;

public static class DependencyInjection
{
    public static IServiceCollection AddPdf(this IServiceCollection services)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddScoped<IPdfEventService, PdfEventService>();

        return services;
    }
}