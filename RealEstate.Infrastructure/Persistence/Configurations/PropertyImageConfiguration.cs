using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Infrastructure.Persistence.Configurations
{
    public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
    {
        public void Configure(EntityTypeBuilder<PropertyImage> builder)
        {
            builder.ToTable("property_images");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.PropertyId)
                .HasColumnName("property_id");

            builder.Property(pi => pi.Url)
                .HasMaxLength(1000)
                .IsRequired()
                .HasColumnName("url");

            builder.Property(pi => pi.StorageProvider)
                .HasConversion<string>()
                .HasDefaultValue(StorageProvider.S3)
                .HasColumnName("storage_provider");

            builder.Property(pi => pi.AltText)
                .HasMaxLength(200)
                .HasColumnName("alt_text");

            builder.Property(pi => pi.IsPrimary)
                .HasDefaultValue(false)
                .HasColumnName("is_primary");

            builder.Property(pi => pi.SortOrder)
                .HasDefaultValue(0)
                .HasColumnName("sort_order");

            builder.Property(pi => pi.Enabled)
                .HasDefaultValue(true)
                .HasColumnName("enabled");

            builder.Property(pi => pi.Checksum)
                .HasMaxLength(64)
                .HasColumnName("checksum");

            builder.Property(pi => pi.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("created_at");

            builder.Property(pi => pi.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("updated_at");

            builder.Property(pi => pi.DeletedAt)
                .HasColumnName("deleted_at");

            builder.Property(pi => pi.RowVersion)
                .HasDefaultValue(1)
                .HasColumnName("row_version");

            // Indexes
            builder.HasIndex(pi => pi.PropertyId)
                .HasDatabaseName("idx_property_images_property");

            builder.HasIndex(pi => new { pi.PropertyId, pi.IsPrimary })
                .HasDatabaseName("idx_property_images_primary");

            builder.HasIndex(pi => new { pi.PropertyId, pi.SortOrder, pi.Enabled })
                .HasDatabaseName("idx_property_images_sorting");

            builder.HasIndex(pi => pi.Enabled)
                .HasDatabaseName("idx_property_images_enabled");

            // Relationships
            builder.HasOne<Property>()
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
