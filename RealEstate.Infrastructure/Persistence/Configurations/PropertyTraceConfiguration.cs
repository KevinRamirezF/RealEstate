using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Infrastructure.Persistence.Configurations
{
    public class PropertyTraceConfiguration : IEntityTypeConfiguration<PropertyTrace>
    {
        public void Configure(EntityTypeBuilder<PropertyTrace> builder)
        {
            builder.ToTable("property_traces");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.PropertyId)
                .HasColumnName("property_id");

            builder.Property(pt => pt.EventType)
                .HasConversion<string>()
                .HasColumnName("event_type");

            builder.Property(pt => pt.EventDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("event_date");

            builder.Property(pt => pt.ActorName)
                .HasMaxLength(180)
                .HasColumnName("actor_name");

            builder.Property(pt => pt.OldValue)
                .HasColumnType("decimal(14,2)")
                .HasColumnName("old_value");

            builder.Property(pt => pt.NewValue)
                .HasColumnType("decimal(14,2)")
                .HasColumnName("new_value");

            builder.Property(pt => pt.TaxAmount)
                .HasColumnType("decimal(14,2)")
                .HasColumnName("tax_amount");

            builder.Property(pt => pt.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");

            builder.Property(pt => pt.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("created_at");

            // Indexes
            builder.HasIndex(pt => new { pt.PropertyId, pt.EventDate })
                .HasDatabaseName("idx_property_traces_property");

            builder.HasIndex(pt => pt.EventType)
                .HasDatabaseName("idx_property_traces_type");

            // Relationships
            builder.HasOne<Property>()
                .WithMany(p => p.Traces)
                .HasForeignKey(pt => pt.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
