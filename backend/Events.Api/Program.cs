using Events;
using Events.Application;
using Events.Domain;
using Events.Pdf;
using Events.Persistence.Linq2db;
using Events.Presentation;
using Events.Presentation.Authentication;
using Events.Publisher;
using Events.Publisher.Rabbit;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
builder.Services
    .AddService()
    .AddDomain()
    // .AddPersistenceMemory()
    // .AddPersistencePsql(builder.Configuration)
    .AddPersistenceLinq2db(builder.Configuration)
    .AddPdf()
    .AddPresentation();
// .AddRabbitMq(builder.Configuration);

builder.Services.AddScoped<IPublisher, DefaultPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

internal class DefaultPublisher : IPublisher
{
    public Task PublishAsync(INotification notification, CancellationToken cancellationToken) => Task.CompletedTask;
}