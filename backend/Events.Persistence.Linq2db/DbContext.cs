using Events.Application;
using Events.Domain.Entities;
using Events.Publisher;
using LinqToDB;
using LinqToDB.Data;

namespace Events.Persistence.Linq2db;

internal class DbContext : DataConnection, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public DbContext(DataOptions options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
        this.CreateTable<Event>(tableOptions: TableOptions.CheckExistence);
    }

    public ITable<Event> Events => this.GetTable<Event>();

    private readonly List<DomainEventsAggregate> _trackedAggregates = [];

    public void Track(DomainEventsAggregate aggregate) => _trackedAggregates.Add(aggregate);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = _trackedAggregates.SelectMany(aggregate => aggregate.DomainEvents).ToList();
        _trackedAggregates.ForEach(a => a.ClearDomainEvents());
        _trackedAggregates.Clear();

        var tasks = domainEvents.Select(e => _publisher.PublishAsync(e, cancellationToken));

        await Task.WhenAll(tasks);

        if (Transaction is not null)
        {
            await base.CommitTransactionAsync(cancellationToken);
        }
    }
}