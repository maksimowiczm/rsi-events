using Events.Domain.Entities;
using Events.Domain.Repositories;
using LinqToDB;

namespace Events.Persistence.Linq2db.Repositories;

internal class EventRepository(DbContext context) : IEventRepository
{
    private void Track(Event? @event)
    {
        if (@event is not null)
        {
            context.Track(@event);
        }
    }

    private void Track(List<Event> events) => events.ForEach(context.Track);

    public Event? GetEvent(Guid id)
    {
        var evt = context.Events.SingleOrDefault(e => e.Id == id);
        Track(evt);
        return evt;
    }

    public IEnumerable<Event> GetEvents()
    {
        var events = context.Events.ToList();
        Track(events);
        return events;
    }

    public void AddEvent(Event @event)
    {
        context.BeginTransaction();
        context.Insert(@event);
        Track(@event);
    }

    public void UpdateEvent(Event @event)
    {
        context.BeginTransaction();
        context.Update(@event);
        Track(@event);
    }

    public IEnumerable<Event> GetEventsBetweenDates(DateTime start, DateTime end)
    {
        var events = context.Events.Where(e => e.Date >= start && e.Date <= end).ToList();
        Track(events);
        return events;
    }

    public IEnumerable<Event> GetEventsByTitle(string title)
    {
        var events = context.Events.Where(e => e.Title == title).ToList();
        Track(events);
        return events;
    }

    public IEnumerable<Event> GetEventsByTitleBetweenDates(string title, DateTime start, DateTime end)
    {
        var events = context.Events.Where(e => e.Title == title && e.Date >= start && e.Date <= end).ToList();
        Track(events);
        return events;
    }
}