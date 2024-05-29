using Events.Application;
using Events.Domain;
using Events.Persistence;
using Events.Publisher.Rabbit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddService()
    .AddDomain()
    .AddPersistence()
    .AddRabbitMq(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/events", (EventService service, DateTime? start, DateTime? end) =>
{
    if (start is not null && end is not null)
    {
        return service.GetEventsBetweenDates(start.Value, end.Value);
    }

    return service.GetEvents();
});

app.MapGet("/events/{id:guid}", (EventService service, Guid id) =>
{
    var dto = service.GetEvent(id);
    return dto is not null ? Results.Ok(dto) : Results.NotFound();
});

app.MapPost("/events",
    async (EventService service, string title, string description, string type, DateTime date) =>
    {
        var dto = await service.CreateEventAsync(title, description, type, date);
        return Results.Created($"/events/{dto.Id}", dto);
    }
);

app.MapPut("/events/{id:guid}",
    async (EventService service, Guid id, string? title, string? description, string? type, DateTime? date) =>
    {
        var result = await service.UpdateEventAsync(id, title, description, type, date);
        return result ? Results.NoContent() : Results.NotFound();
    });

app.Run();