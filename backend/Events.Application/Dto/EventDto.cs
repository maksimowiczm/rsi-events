using Events.Domain.Entities;

namespace Events.Application.Dto;

public record EventDto(Guid Id, string Title, string Description, string Type, string Date);

internal static class EventMapper
{
    public static EventDto MapToDto(this Event @event) =>
        new(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.EventType,
            @event.Date.ToUniversalTime().ToShortDateString()
        );
}