using Events.Application.Abstractions;
using Events.Application.Dto;
using Events.Domain.Entities;
using Events.Domain.Repositories;

namespace Events.Application;

public class EventService(
    EventFactory eventFactory,
    IEventRepository eventRepository,
    IUnitOfWork unitOfWork,
    IPdfEventService pdfEventService
)
{
    public async Task<EventDto> CreateEventAsync(
        string title,
        string description,
        string eventType,
        DateTime time,
        CancellationToken cancellationToken = default
    )
    {
        var @event = eventFactory.CreateEvent(title, description, eventType, time);
        eventRepository.AddEvent(@event);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return @event.MapToDto();
    }

    public async Task<bool> UpdateEventAsync(
        Guid id,
        string? title,
        string? description,
        string? eventType,
        DateTime? date,
        CancellationToken cancellationToken = default
    )
    {
        var @event = eventRepository.GetEvent(id);
        if (@event is not null)
        {
            @event.Update(title, description, eventType, date);
            eventRepository.UpdateEvent(@event);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    public async Task<EventDto?> GetEventAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var @event = eventRepository.GetEvent(id);
        if (@event is null)
        {
            return null;
        }

        @event.Visit();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return @event.MapToDto();
    }

    public async Task<IEnumerable<EventDto>> GetEventsAsync(CancellationToken cancellationToken = default)
    {
        var events = eventRepository.GetEvents().ToList();
        events.ForEach(e => e.Visit());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return events.Select(e => e.MapToDto());
    }

    public async Task<IEnumerable<EventDto>> GetEventsByTitleAsync(
        string title,
        CancellationToken cancellationToken = default
    )
    {
        var events = eventRepository.GetEventsByTitle(title).ToList();
        events.ForEach(e => e.Visit());
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return events.Select(e => e.MapToDto());
    }

    public async Task<IEnumerable<EventDto>> GetEventsBetweenDatesAsync(
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default
    )
    {
        var events = eventRepository.GetEventsBetweenDates(start, end).ToList();
        events.ForEach(e => e.Visit());
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return events.Select(e => e.MapToDto());
    }

    public async Task<IEnumerable<EventDto>> GetEventsByTitleBetweenDatesAsync(
        string title,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken = default
    )
    {
        var events = eventRepository.GetEventsByTitleBetweenDates(title, start, end).ToList();
        events.ForEach(e => e.Visit());
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return events.Select(e => e.MapToDto());
    }

    public Task<byte[]?> GetEventPdfAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var @event = eventRepository.GetEvent(id);
        if (@event is null)
        {
            return Task.FromResult<byte[]?>(null);
        }

        var pdf = pdfEventService.Generate(@event);

        return Task.FromResult<byte[]?>(pdf);
    }

    public Task<byte[]> GetEventsPdfAsync(
        DateTime? start,
        DateTime? end,
        CancellationToken cancellationToken = default
    )
    {
        var events = (start, end) switch
        {
            (not null, not null) => eventRepository.GetEventsBetweenDates(start.Value, end.Value),
            _ => eventRepository.GetEvents()
        };

        var pdf = pdfEventService.Generate(events);

        return Task.FromResult(pdf);
    }

    public async Task<bool> DeleteEventAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var @event = eventRepository.GetEvent(id);
        if (@event is null)
        {
            return false;
        }

        eventRepository.DeleteEvent(@event);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}