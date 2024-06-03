using Events.Domain.Entities;
using Events.Domain.Repositories;

namespace Events.Persistence.Psql.Repositories;

public class EventRepository(EventsDbContext context) : IEventRepository
{
    public Event? GetEvent(Guid id) => context.Events.Find(id);

    public IEnumerable<Event> GetEvents() => context.Events;

    public void AddEvent(Event @event) => context.Events.Add(@event);

    public void UpdateEvent(Event @event) => context.Events.Update(@event);

    public void DeleteEvent(Event @event) => context.Events.Remove(@event);

    public IEnumerable<Event> GetEventsBetweenDates(DateTime start, DateTime end) =>
        context.Events.Where(e => e.Date >= start && e.Date <= end);

    public IEnumerable<Event> GetEventsByTitle(string title) =>
        context.Events.Where(e => e.Title == title);

    public IEnumerable<Event> GetEventsByTitleBetweenDates(string title, DateTime start, DateTime end) =>
        context.Events.Where(e => e.Title == title && e.Date >= start && e.Date <= end);
}