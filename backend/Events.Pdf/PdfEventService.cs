using Events.Application.Abstractions;
using Events.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Events.Pdf;

public class PdfEventService : IPdfEventService
{
    private Action<PageDescriptor> GeneratePage(Event evt)
    {
        var header = $"{evt.Title} - {evt.Date:dd/MM/yyyy}";
        var content = $"""
                       {evt.EventType}

                       Description:
                       {evt.Description}
                       """;

        return p =>
        {
            p.Size(PageSizes.A4);
            p.Margin(2, Unit.Centimetre);
            p.PageColor(Colors.White);
            p.DefaultTextStyle(t => t.FontSize(20));

            p
                .Header()
                .Text(header)
                .Bold();

            p
                .Content()
                .Text(content)
                .FontSize(12);
        };
    }

    public byte[] Generate(Event evt)
    {
        var pdf = Document.Create(c => c.Page(GeneratePage(evt))).GeneratePdf();

        return pdf;
    }

    public byte[] Generate(IEnumerable<Event> events)
    {
        var pdf = Document
            .Create(c =>
            {
                foreach (var evt in events)
                {
                    c.Page(GeneratePage(evt));
                }
            })
            .GeneratePdf();

        return pdf;
    }
}