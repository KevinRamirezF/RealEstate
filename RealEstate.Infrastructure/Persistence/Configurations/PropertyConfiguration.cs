using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Infrastructure.Persistence.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.ToTable("properties", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_properties_price", "price >= 0");
                tableBuilder.HasCheckConstraint("CK_properties_bathrooms", "bathrooms >= 0");
                tableBuilder.HasCheckConstraint("CK_properties_bedrooms", "bedrooms >= 0");
            });

            builder.HasKey(p => p.Id);

            builder.Property(p => p.OwnerId)
                .HasColumnName("owner_id");

            builder.Property(p => p.CodeInternal)
                .HasMaxLength(40)
                .IsRequired()
                .HasColumnName("code_internal");

            builder.Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired()
                .HasColumnName("name");

            builder.Property(p => p.Description)
                .HasColumnType("text")
                .HasColumnName("description");

            builder.Property(p => p.PropertyType)
                .HasConversion<string>()
                .HasColumnName("property_type");

            builder.Property(p => p.YearBuilt)
                .HasColumnName("year_built");

            builder.Property(p => p.Bedrooms)
                .HasDefaultValue(0)
                .HasColumnName("bedrooms");

            builder.Property(p => p.Bathrooms)
                .HasColumnType("decimal(4,1)")
                .HasDefaultValue(0.0m)
                .HasColumnName("bathrooms");

            builder.Property(p => p.ParkingSpaces)
                .HasDefaultValue(0)
                .HasColumnName("parking_spaces");

            builder.Property(p => p.AreaSqft)
                .HasColumnName("area_sqft");

            builder.Property(p => p.LotSizeSqft)
                .HasColumnName("lot_size_sqft");

            builder.Property(p => p.Price)
                .HasColumnType("decimal(14,2)")
                .IsRequired()
                .HasColumnName("price");

            builder.Property(p => p.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("USD")
                .HasColumnName("currency");

            builder.Property(p => p.HoaFee)
                .HasColumnType("decimal(12,2)")
                .HasColumnName("hoa_fee");

            builder.Property(p => p.AddressLine)
                .HasMaxLength(200)
                .IsRequired()
                .HasColumnName("address_line");

            builder.Property(p => p.City)
                .HasMaxLength(120)
                .IsRequired()
                .HasColumnName("city");

            builder.Property(p => p.State)
                .HasMaxLength(2)
                .IsRequired()
                .HasColumnName("state");

            builder.Property(p => p.PostalCode)
                .HasMaxLength(10)
                .IsRequired()
                .HasColumnName("postal_code");

            builder.Property(p => p.Country)
                .HasMaxLength(2)
                .HasDefaultValue("US")
                .HasColumnName("country");

            builder.Property(p => p.Lat)
                .HasColumnType("decimal(10,7)")
                .HasColumnName("lat");

            builder.Property(p => p.Lng)
                .HasColumnType("decimal(10,7)")
                .HasColumnName("lng");

            builder.Property(p => p.ListingStatus)
                .HasConversion<string>()
                .HasDefaultValue(ListingStatus.ACTIVE)
                .HasColumnName("listing_status");

            builder.Property(p => p.ListingDate)
                .HasDefaultValueSql("CAST(GETDATE() AS DATE)")
                .HasColumnName("listing_date");

            builder.Property(p => p.LastSoldPrice)
                .HasColumnType("decimal(14,2)")
                .HasColumnName("last_sold_price");

            builder.Property(p => p.IsFeatured)
                .HasDefaultValue(false)
                .HasColumnName("is_featured");

            builder.Property(p => p.IsPublished)
                .HasDefaultValue(true)
                .HasColumnName("is_published");

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("created_at");

            builder.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("updated_at");

            builder.Property(p => p.DeletedAt)
                .HasColumnName("deleted_at");

            builder.Property(p => p.RowVersion)
                .HasDefaultValue(1)
                .HasColumnName("row_version");

            // Check constraints moved to ToTable configuration

            // Indexes
            builder.HasIndex(p => p.CodeInternal)
                .IsUnique()
                .HasDatabaseName("idx_properties_code_internal");

            builder.HasIndex(p => p.OwnerId)
                .HasDatabaseName("idx_properties_owner");

            builder.HasIndex(p => p.Price)
                .HasDatabaseName("idx_properties_price");

            builder.HasIndex(p => new { p.State, p.City, p.PostalCode })
                .HasDatabaseName("idx_properties_geo");

            builder.HasIndex(p => new { p.PropertyType, p.ListingStatus })
                .HasDatabaseName("idx_properties_type_status");

            builder.HasIndex(p => p.CreatedAt)
                .HasDatabaseName("idx_properties_created_at");

            builder.HasIndex(p => p.IsFeatured)
                .HasDatabaseName("idx_properties_featured");

            builder.HasIndex(p => new { p.IsPublished, p.ListingStatus })
                .HasDatabaseName("idx_properties_published_status");

            builder.HasIndex(p => p.YearBuilt)
                .HasDatabaseName("idx_properties_year_built");

            builder.HasIndex(p => p.Bedrooms)
                .HasDatabaseName("idx_properties_bedrooms");

            builder.HasIndex(p => p.AreaSqft)
                .HasDatabaseName("idx_properties_area");

            // Relationships
            builder.HasOne<Owner>()
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Images)
                .WithOne()
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Traces)
                .WithOne()
                .HasForeignKey(pt => pt.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
