using Events.Application;
using Events.Domain;
using Events.Pdf;
using Events.Persistence.Linq2db;
using Events.Presentation;
using Events.Publisher.Rabbit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services
    .AddService()
    .AddDomain()
    // .AddPersistenceMemory()
    // .AddPersistencePsql(builder.Configuration)
    .AddPersistenceLinq2db(builder.Configuration)
    .AddPdf()
    .AddPresentation()
    .AddRabbitMq(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();