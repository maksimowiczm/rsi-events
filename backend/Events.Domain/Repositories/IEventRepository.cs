using Events.Domain.Entities;

namespace Events.Domain.Repositories;

public interface IEventRepository
{
    Event? GetEvent(Guid id);

    IEnumerable<Event> GetEvents();

    void AddEvent(Event @event);

    void UpdateEvent(Event @event);

    void DeleteEvent(Event @event);

    IEnumerable<Event> GetEventsBetweenDates(DateTime start, DateTime end);

    IEnumerable<Event> GetEventsByTitle(string title);

    IEnumerable<Event> GetEventsByTitleBetweenDates(string title, DateTime start, DateTime end);
}