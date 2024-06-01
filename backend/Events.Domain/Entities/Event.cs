using Events.Domain.Events;

namespace Events.Domain.Entities;

public class Event : DomainEventsAggregate
{
    private Event(string title, string description, string eventType, DateTime date)
    {
        Title = title;
        Description = description;
        EventType = eventType;
        Date = date;
    }

    internal static Event Create(string title, string description, string eventType, DateTime date)
    {
        var evt = new Event(title, description, eventType, date);
        evt.Raise(new NewEventCreated(title, date));
        return evt;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public string Title { get; private set; }

    public string Description { get; private set; }

    public string EventType { get; private set; }

    public DateTime Date { get; private set; }

    public void Update(string? title, string? description, string? eventType, DateTime? date)
    {
        Title = title ?? Title;
        Description = description ?? Description;
        EventType = eventType ?? EventType;
        Date = date ?? Date;
    }

    public void Visit() => Raise(new EventVisited(Id));
}