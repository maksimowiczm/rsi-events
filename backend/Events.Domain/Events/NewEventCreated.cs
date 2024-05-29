using Events.Publisher;

namespace Events.Domain.Events;

public record NewEventCreated(string Title, DateTime Date) : INotification;