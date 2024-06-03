using Events.Application;
using Events.Application.Dto;
using Events.Presentation.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiskFirst.Hateoas;

namespace Events.Presentation.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class EventsController(EventService service, ILinksService link) : ControllerBase
{
    [HttpGet(Name = "GetAllEventsRoute")]
    public async Task<IEnumerable<EventDto>> GetEvents(
        DateTime? start,
        DateTime? end,
        string? title,
        CancellationToken ct
    )
    {
        var eventsTask = (title, start, end) switch
        {
            ({ }, null, null) => service.GetEventsByTitleAsync(title, ct),
            (null, { }, { }) => service.GetEventsBetweenDatesAsync(start.Value, end.Value, ct),
            ({ }, { }, { }) => service.GetEventsByTitleBetweenDatesAsync(title, start.Value, end.Value, ct),
            _ => service.GetEventsAsync(ct),
        };

        var events = (await eventsTask).ToList();
        await Task.WhenAll(events.Select(link.AddLinksAsync));
        return events;
    }

    [HttpGet("{id:guid}", Name = "GetEventByIdRoute")]
    public async Task<ActionResult<EventDto>> GetEvent(Guid id, CancellationToken cancellationToken)
    {
        var dto = await service.GetEventAsync(id, cancellationToken);
        if (dto is null)
        {
            return NotFound();
        }

        await link.AddLinksAsync(dto);

        return dto;
    }

    [HttpPost]
    public async Task<ActionResult<EventDto>> CreateEvent(
        [FromBody] CreateEventRequest request,
        CancellationToken cancellationToken
    )
    {
        var dto = await service.CreateEventAsync(
            request.Title,
            request.Description,
            request.Type,
            request.Date,
            cancellationToken
        );

        await link.AddLinksAsync(dto);

        return CreatedAtAction(nameof(GetEvent), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEvent(
        Guid id,
        [FromBody] UpdateEventRequest request,
        CancellationToken cancellationToken
    )
    {
        var dto = await service.UpdateEventAsync(
            id,
            request.Title,
            request.Description,
            request.Type,
            request.Date,
            cancellationToken
        );

        return dto ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteEvent(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteEventAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> GetEventPdf(Guid id, CancellationToken cancellationToken)
    {
        var pdf = await service.GetEventPdfAsync(id, cancellationToken);
        if (pdf is null)
        {
            return NotFound();
        }

        return File(pdf, "application/pdf");
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> GetEventsPdf(DateTime? start, DateTime? end, CancellationToken cancellationToken)
    {
        var pdf = await service.GetEventsPdfAsync(start, end, cancellationToken);
        return File(pdf, "application/pdf");
    }
}

public record CreateEventRequest(string Title, string Description, string Type, DateTime Date);

public record UpdateEventRequest(string? Title, string? Description, string? Type, DateTime? Date);