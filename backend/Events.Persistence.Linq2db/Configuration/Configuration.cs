using LinqToDB.Mapping;

namespace Events.Persistence.Linq2db.Configuration;

internal static class Configuration
{
    public static FluentMappingBuilder BuildSchema(this FluentMappingBuilder builder)
    {
        builder.BuildEvent();

        return builder;
    }
}