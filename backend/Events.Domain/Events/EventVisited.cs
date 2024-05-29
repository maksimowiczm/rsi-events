using Events.Publisher;

namespace Events.Domain.Events;

public record EventVisited(Guid Id) : INotification;