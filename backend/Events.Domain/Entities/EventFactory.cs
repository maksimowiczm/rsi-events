namespace Events.Domain.Entities;

public class EventFactory
{
    public Event CreateEvent(string title, string description, string eventType, DateTime date) =>
        Event.Create(title, description, eventType, date);
}