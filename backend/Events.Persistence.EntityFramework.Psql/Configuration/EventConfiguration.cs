using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Events.Persistence.Psql.Configuration;

internal sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.Title)
            .HasMaxLength(50);

        builder
            .Property(e => e.Description)
            .HasMaxLength(150);

        builder
            .Property(e => e.Date)
            .HasPrecision(0);

        builder
            .Property(e => e.EventType)
            .HasMaxLength(25);
    }
}