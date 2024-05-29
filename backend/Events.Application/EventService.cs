using Events.Application.Dto;
using Events.Domain.Entities;
using Events.Domain.Repositories;

namespace Events.Application;

public class EventService
{
    private readonly EventFactory _eventFactory;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EventService(EventFactory eventFactory, IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventFactory = eventFactory;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventDto> CreateEventAsync(string title, string description, string eventType, DateTime time)
    {
        var @event = _eventFactory.CreateEvent(title, description, eventType, time);
        _eventRepository.AddEvent(@event);
        await _unitOfWork.SaveChangesAsync();
        return @event.MapToDto();
    }

    public async Task<bool> UpdateEventAsync(
        Guid id,
        string? title,
        string? description,
        string? eventType,
        DateTime? date
    )
    {
        var @event = _eventRepository.GetEvent(id);
        if (@event is not null)
        {
            @event.Update(title, description, eventType, date);
            _eventRepository.UpdateEvent(@event);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<EventDto?> GetEventAsync(Guid id)
    {
        var @event = _eventRepository.GetEvent(id);
        if (@event is null)
        {
            return null;
        }

        @event.Visit();
        await _unitOfWork.SaveChangesAsync();

        return @event.MapToDto();
    }

    public async Task<IEnumerable<EventDto>> GetEventsAsync()
    {
        var events = _eventRepository.GetEvents().ToList();
        events.ForEach(e => e.Visit());
        await _unitOfWork.SaveChangesAsync();

        return events.Select(e => e.MapToDto());
    }

    public async Task<IEnumerable<EventDto>> GetEventsBetweenDatesAsync(DateTime start, DateTime end)
    {
        var events = _eventRepository.GetEventsBetweenDates(start, end).ToList();
        events.ForEach(e => e.Visit());
        await _unitOfWork.SaveChangesAsync();
        return events.Select(e => e.MapToDto());
    }
}