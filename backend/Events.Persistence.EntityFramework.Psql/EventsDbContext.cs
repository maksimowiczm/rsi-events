using System.Reflection;
using Events.Application.Abstractions;
using Events.Domain.Entities;
using Events.Publisher;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

namespace Events.Persistence.Psql;

public class EventsDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public DbSet<Event> Events { get; set; }

    public EventsDbContext(IPublisher publisher, DbContextOptions<EventsDbContext> options) : base(options)
    {
        _publisher = publisher;
    }

    public EventsDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    async Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        var entities = ChangeTracker
            .Entries<DomainEventsAggregate>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents);

        entities.ForEach(e => e.ClearDomainEvents());
        var tasks = events.Select(e => _publisher.PublishAsync(e, cancellationToken));

        await Task.WhenAll(tasks);

        await base.SaveChangesAsync(cancellationToken);
    }
}