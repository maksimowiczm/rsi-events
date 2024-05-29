using Events.Domain.Entities;
using Events.Domain.Repositories;

namespace Events.Persistence;

public class EventRepository : IEventRepository
{
    private readonly List<Event> _events;

    public EventRepository(EventFactory factory)
    {
        _events =
        [
            factory.CreateEvent("Event 1", "Description 1", "Type 1", DateTime.Now),
            factory.CreateEvent("Event 2", "Description 2", "Type 2", DateTime.Now.AddDays(-1)),
            factory.CreateEvent("Event 3", "Description 3", "Type 3", DateTime.Now.AddDays(7))
        ];

        foreach (var e in _events)
        {
            e.ClearDomainEvents();
        }
    }

    public Event? GetEvent(Guid id) => _events.FirstOrDefault(e => e.Id == id);

    public IEnumerable<Event> GetEvents() => _events.AsEnumerable();

    public void AddEvent(Event @event) => _events.Add(@event);

    public void UpdateEvent(Event @event)
    {
        var index = _events.FindIndex(e => e.Id == @event.Id);
        if (index >= 0)
        {
            _events[index] = @event;
        }
    }

    public IEnumerable<Event> GetEventsBetweenDates(DateTime start, DateTime end) =>
        _events.Where(e => e.Date >= start && e.Date <= end);
}