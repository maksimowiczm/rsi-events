using Events.Application;
using Events.Domain;
using Events.Pdf;
using Events.Persistence.Linq2db;
using Events.Publisher.Rabbit;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddService()
    .AddDomain()
    // .AddPersistenceMemory()
    // .AddPersistencePsql(builder.Configuration)
    .AddPersistenceLinq2db(builder.Configuration)
    .AddPdf()
    .AddRabbitMq(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/api/events",
    (EventService service, DateTime? start, DateTime? end, string? title, CancellationToken cancellationToken) =>
    {
        // bruh

        if (title is not null)
        {
            if (start is not null && end is not null)
            {
                return service.GetEventsByTitleBetweenDatesAsync(title, start.Value, end.Value, cancellationToken);
            }

            return service.GetEventsByTitleAsync(title, cancellationToken);
        }

        if (start is not null && end is not null)
        {
            return service.GetEventsBetweenDatesAsync(start.Value, end.Value, cancellationToken);
        }

        return service.GetEventsAsync(cancellationToken);
    });

app.MapGet("/api/events/{id:guid}", async (EventService service, Guid id, CancellationToken cancellationToken) =>
{
    var dto = await service.GetEventAsync(id, cancellationToken);
    return dto is not null ? Results.Ok(dto) : Results.NotFound();
});

app.MapGet("/api/events/{id:guid}/pdf",
    async (EventService service, Guid id, CancellationToken cancellationToken) =>
    {
        var pdf = await service.GetEventPdfAsync(id, cancellationToken);
        if (pdf is null)
        {
            return Results.NotFound();
        }

        return Results.Stream(new MemoryStream(pdf), "application/pdf");
    });

app.MapGet("/api/events/pdf",
    async (EventService service, DateTime? start, DateTime? end, CancellationToken cancellationToken) =>
    {
        var pdf = await service.GetEventsPdfAsync(start, end, cancellationToken);
        return Results.Stream(new MemoryStream(pdf), "application/pdf");
    });

app.MapPost("/api/events",
    async (EventService service, [FromBody] CreateEventRequest request, CancellationToken cancellationToken) =>
    {
        var dto = await service.CreateEventAsync(
            request.Title,
            request.Description,
            request.Type,
            request.Date,
            cancellationToken
        );
        return Results.Created($"/events/{dto.Id}", dto);
    }
);

app.MapPut("/api/events/{id:guid}",
    async (EventService service, Guid id, [FromBody] UpdateEventRequest request, CancellationToken cancellationToken) =>
    {
        var result = await service.UpdateEventAsync(id,
            request.Title,
            request.Description,
            request.Type,
            request.Date,
            cancellationToken
        );
        return result ? Results.NoContent() : Results.NotFound();
    });

app.Run();

record CreateEventRequest(string Title, string Description, string Type, DateTime Date);

record UpdateEventRequest(string? Title, string? Description, string? Type, DateTime? Date);