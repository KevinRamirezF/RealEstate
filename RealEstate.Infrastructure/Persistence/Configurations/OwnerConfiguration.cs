using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations
{
    public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
    {
        public void Configure(EntityTypeBuilder<Owner> builder)
        {
            builder.ToTable("owners");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.ExternalCode)
                .HasMaxLength(40)
                .HasColumnName("external_code");

            builder.Property(o => o.FullName)
                .HasMaxLength(180)
                .IsRequired()
                .HasColumnName("full_name");

            builder.Property(o => o.Email)
                .HasMaxLength(180)
                .HasColumnName("email");

            builder.Property(o => o.Phone)
                .HasMaxLength(40)
                .HasColumnName("phone");

            builder.Property(o => o.PhotoUrl)
                .HasMaxLength(500)
                .HasColumnName("photo_url");

            builder.Property(o => o.BirthDate)
                .HasColumnName("birth_date");

            builder.Property(o => o.AddressLine)
                .HasMaxLength(200)
                .HasColumnName("address_line");

            builder.Property(o => o.City)
                .HasMaxLength(120)
                .HasColumnName("city");

            builder.Property(o => o.State)
                .HasMaxLength(2)
                .HasColumnName("state");

            builder.Property(o => o.PostalCode)
                .HasMaxLength(10)
                .HasColumnName("postal_code");

            builder.Property(o => o.Country)
                .HasMaxLength(2)
                .HasDefaultValue("US")
                .HasColumnName("country");

            builder.Property(o => o.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");

            builder.Property(o => o.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("created_at");

            builder.Property(o => o.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnName("updated_at");

            builder.Property(o => o.DeletedAt)
                .HasColumnName("deleted_at");

            builder.Property(o => o.RowVersion)
                .HasDefaultValue(1)
                .HasColumnName("row_version")
                .IsConcurrencyToken();

            // Indexes
            builder.HasIndex(o => o.FullName)
                .HasDatabaseName("idx_owners_name");

            builder.HasIndex(o => o.Email)
                .IsUnique()
                .HasDatabaseName("idx_owners_email");

            builder.HasIndex(o => o.ExternalCode)
                .IsUnique()
                .HasDatabaseName("idx_owners_external_code");

            builder.HasIndex(o => o.IsActive)
                .HasDatabaseName("idx_owners_active");

            builder.HasIndex(o => new { o.State, o.City })
                .HasDatabaseName("idx_owners_location");

            builder.HasIndex(o => new { o.IsActive, o.CreatedAt })
                .HasDatabaseName("idx_owners_active_created");

            builder.HasIndex(o => o.CreatedAt)
                .HasDatabaseName("idx_owners_created_at");
        }
    }
}
