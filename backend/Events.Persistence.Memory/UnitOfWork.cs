using Events.Application.Abstractions;
using Events.Domain.Repositories;
using Events.Publisher;

namespace Events.Persistence;

public class UnitOfWork(IEventRepository eventsRepository, IPublisher publisher) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = eventsRepository.GetEvents();

        foreach (var entity in entities)
        {
            var domainEvents = entity.DomainEvents;
            foreach (var notification in domainEvents)
            {
                await publisher.PublishAsync(notification, cancellationToken);
            }

            entity.ClearDomainEvents();
        }
    }
}