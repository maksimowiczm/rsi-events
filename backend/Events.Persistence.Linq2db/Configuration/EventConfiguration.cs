using Events.Domain.Entities;
using LinqToDB.Mapping;

namespace Events.Persistence.Linq2db.Configuration;

internal static class EventConfiguration
{
    public static FluentMappingBuilder BuildEvent(this FluentMappingBuilder builder)
    {
        builder
            .Entity<Event>()
            .HasTableName("events")
            .HasPrimaryKey(e => e.Id)
            // title
            .Property(e => e.Title)
            .HasLength(50)
            // description
            .Property(e => e.Description)
            .HasLength(150)
            // date
            .Property(e => e.Date)
            .HasPrecision(0)
            // eventType
            .Property(e => e.EventType)
            .HasLength(25);

        return builder.Build();
    }
}