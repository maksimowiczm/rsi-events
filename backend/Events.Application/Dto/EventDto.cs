using Events.Domain.Entities;
using RiskFirst.Hateoas.Models;

namespace Events.Application.Dto;

public class EventDto(Guid id, string title, string description, string type, string date) : LinkContainer
{
    public Guid Id { get; init; } = id;
    public string Title { get; init; } = title;
    public string Description { get; init; } = description;
    public string Type { get; init; } = type;
    public string Date { get; init; } = date;
}

internal static class EventMapper
{
    public static EventDto MapToDto(this Event @event) =>
        new(
            @event.Id,
            @event.Title,
            @event.Description,
            @event.EventType,
            @event.Date.ToUniversalTime().ToString("yyyy-MM-dd")
        );
}