using Events.Domain.Entities;

namespace Events.Domain.Repositories;

public interface IEventRepository
{
    Event? GetEvent(Guid id);

    IEnumerable<Event> GetEvents();

    void AddEvent(Event @event);

    void UpdateEvent(Event @event);

    IEnumerable<Event> GetEventsBetweenDates(DateTime start, DateTime end);
}