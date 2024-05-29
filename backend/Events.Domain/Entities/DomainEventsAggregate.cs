using Events.Publisher;

namespace Events.Domain.Entities;

public abstract class DomainEventsAggregate
{
    private readonly List<INotification> _domainEvents = [];

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents;

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void Raise(INotification domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}