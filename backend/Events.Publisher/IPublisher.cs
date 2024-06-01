namespace Events.Publisher;

public interface IPublisher
{
    Task PublishAsync(INotification notification, CancellationToken cancellationToken);
}