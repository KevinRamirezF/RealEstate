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
                tableBuilder.HasCheckConstraint("CK_properties_base_price", "base_price >= 0");
                tableBuilder.HasCheckConstraint("CK_properties_tax_amount", "tax_amount >= 0");
                tableBuilder.HasCheckConstraint("CK_properties_price", "price >= 0 AND price = base_price + tax_amount");
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
                .HasDefaultValue(0)
                .HasColumnName("bathrooms");

            builder.Property(p => p.ParkingSpaces)
                .HasDefaultValue(0)
                .HasColumnName("parking_spaces");

            builder.Property(p => p.AreaSqft)
                .HasColumnName("area_sqft");

            builder.Property(p => p.BasePrice)
                .HasColumnType("decimal(14,2)")
                .IsRequired()
                .HasColumnName("base_price");

            builder.Property(p => p.TaxAmount)
                .HasColumnType("decimal(14,2)")
                .IsRequired()
                .HasColumnName("tax_amount");

            builder.Property(p => p.Price)
                .HasColumnType("decimal(14,2)")
                .IsRequired()
                .HasColumnName("price");

            builder.Property(p => p.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("USD")
                .HasColumnName("currency");


            builder.Property(p => p.AddressLine)
                .HasMaxLength(200)
                .IsRequired()
                .HasColumnName("address_line");

            builder.Property(p => p.City)
                .HasMaxLength(120)
                .IsRequired()
                .HasColumnName("city");

            builder.Property(p => p.State)
                .HasMaxLength(50)
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
                .HasSentinel(ListingStatus.DRAFT)
                .HasColumnName("listing_status");

            builder.Property(p => p.ListingDate)
                .HasDefaultValueSql("CAST(GETDATE() AS DATE)")
                .HasColumnName("listing_date");


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
                .HasColumnName("row_version")
                .IsConcurrencyToken();

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

            builder.HasIndex(p => p.ListingDate)
                .HasDatabaseName("idx_properties_listing_date");

            builder.HasIndex(p => p.Name)
                .HasDatabaseName("idx_properties_name");

            // Composite indexes for common query patterns
            builder.HasIndex(p => new { p.Price, p.ListingDate })
                .HasDatabaseName("idx_properties_price_listing");

            builder.HasIndex(p => new { p.IsPublished, p.IsFeatured, p.ListingDate })
                .HasDatabaseName("idx_properties_published_featured");

            builder.HasIndex(p => new { p.PropertyType, p.Price, p.Bedrooms })
                .HasDatabaseName("idx_properties_type_price_bedrooms");

            builder.HasIndex(p => new { p.State, p.City, p.Price })
                .HasDatabaseName("idx_properties_location_price");

            builder.HasIndex(p => p.Bathrooms)
                .HasDatabaseName("idx_properties_bathrooms");



            // Geospatial indexes for Lat/Lng searches
            builder.HasIndex(p => new { p.Lat, p.Lng })
                .HasDatabaseName("idx_properties_coordinates");

            // Critical composite indexes for range queries
            builder.HasIndex(p => new { p.Bedrooms, p.Bathrooms, p.Price })
                .HasDatabaseName("idx_properties_beds_baths_price");


            // Ultimate performance composite index
            builder.HasIndex(p => new { p.IsPublished, p.PropertyType, p.ListingStatus, p.State, p.City, p.Price })
                .HasDatabaseName("idx_properties_search_ultimate");

            // ADDITIONAL CRITICAL INDEXES FOR COMPLETE FILTER COVERAGE
            
            // Range query optimization indexes
            builder.HasIndex(p => new { p.Bedrooms, p.Price })
                .HasDatabaseName("idx_properties_bedrooms_price_range");
                
            builder.HasIndex(p => new { p.Bathrooms, p.Price })
                .HasDatabaseName("idx_properties_bathrooms_price_range");
                
            builder.HasIndex(p => new { p.AreaSqft, p.Price })
                .HasDatabaseName("idx_properties_area_price_range");
                
                

            // Geospatial bounding box searches (critical for map views)
            builder.HasIndex(p => new { p.Lat, p.Lng, p.IsPublished, p.ListingStatus })
                .HasDatabaseName("idx_properties_geospatial_published");

            // Primary image availability filter
            builder.HasIndex(p => new { p.IsPublished, p.ListingStatus })
                .HasDatabaseName("idx_properties_published_listing_status");

            // Multi-range composite indexes for complex filtering (optimized for min/max queries)
            builder.HasIndex(p => new { p.PropertyType, p.Bedrooms, p.Bathrooms, p.Price, p.AreaSqft })
                .HasDatabaseName("idx_properties_full_range_search");
                
            // Additional range optimization indexes for better min/max performance
            builder.HasIndex(p => new { p.IsPublished, p.ListingStatus, p.Bedrooms, p.Price })
                .HasDatabaseName("idx_properties_bedrooms_range_opt");
                
            builder.HasIndex(p => new { p.IsPublished, p.ListingStatus, p.Bathrooms, p.Price })
                .HasDatabaseName("idx_properties_bathrooms_range_opt");
                
            builder.HasIndex(p => new { p.IsPublished, p.ListingStatus, p.AreaSqft, p.Price })
                .HasDatabaseName("idx_properties_area_range_opt");
                
            builder.HasIndex(p => new { p.State, p.City, p.PropertyType, p.Price, p.Bedrooms })
                .HasDatabaseName("idx_properties_location_type_specs");

            // Performance indexes for sorting options
            builder.HasIndex(p => new { p.IsPublished, p.Price, p.ListingDate, p.CreatedAt })
                .HasDatabaseName("idx_properties_published_sorted");

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
