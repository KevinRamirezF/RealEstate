using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Identity;
using System.Reflection;

namespace RealEstate.Infrastructure.Persistence
{
    public class RealEstateDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public RealEstateDbContext(DbContextOptions<RealEstateDbContext> options) : base(options)
        {
        }

        public DbSet<Owner> Owners { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<PropertyTrace> PropertyTraces { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Apply entity configurations
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configure enum to string conversions
            builder.Entity<Property>()
                .Property(e => e.PropertyType)
                .HasConversion<string>();

            builder.Entity<Property>()
                .Property(e => e.ListingStatus)
                .HasConversion<string>();

            builder.Entity<PropertyImage>()
                .Property(e => e.StorageProvider)
                .HasConversion<string>();

            builder.Entity<PropertyTrace>()
                .Property(e => e.EventType)
                .HasConversion<string>();

            // Configure soft delete global filters
            builder.Entity<Owner>().HasQueryFilter(e => e.DeletedAt == null);
            builder.Entity<Property>().HasQueryFilter(e => e.DeletedAt == null);
            builder.Entity<PropertyImage>().HasQueryFilter(e => e.DeletedAt == null);

            // Configure concurrency tokens
            builder.Entity<Owner>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken();

            builder.Entity<Property>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken();

            builder.Entity<PropertyImage>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken();
        }
    }
}
