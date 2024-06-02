using Events.Domain.Entities;

namespace Events.Application.Abstractions;

public interface IPdfEventService
{
    public byte[] Generate(Event evt);

    public byte[] Generate(IEnumerable<Event> events);
}