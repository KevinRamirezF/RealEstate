using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Persistence
{
    public static class RealEstateDbContextSeed
    {
        public static async Task SeedExactDataAsync(RealEstateDbContext context)
        {
            if (await context.Owners.AnyAsync())
            {
                return; // DB has been seeded
            }

            // Seed exact data from JSON specification
            var owner = Owner.Create("Jane Doe", "jane@example.com", "+1-555-0100");
            owner.GetType().GetProperty("Id")?.SetValue(owner, Guid.Parse("6b9a7c91-7e6d-4d7e-9b3d-9a0b7a5b7b01"));
            await context.Owners.AddAsync(owner);

            var property = Property.Create(
                Guid.Parse("6b9a7c91-7e6d-4d7e-9b3d-9a0b7a5b7b01"),
                "CA-SF-2025-0001",
                "Modern Condo in SoMa",
                PropertyType.CONDO,
                1090000.0m, // Base price
                109000.0m,  // Tax amount (10%)
                "123 Market St",
                "San Francisco",
                "CA",
                "94103"
            );
            property.GetType().GetProperty("Id")?.SetValue(property, Guid.Parse("3f5b1f8f-6e2a-4d3a-8a2a-439bc8f76b11"));
            property.GetType().GetProperty("Description")?.SetValue(property, "2BR/2BA with parking and city view");
            property.GetType().GetProperty("YearBuilt")?.SetValue(property, (short)2016);
            property.GetType().GetProperty("Bedrooms")?.SetValue(property, (short)2);
            property.GetType().GetProperty("Bathrooms")?.SetValue(property, 2);
            property.GetType().GetProperty("ParkingSpaces")?.SetValue(property, (short)1);
            property.GetType().GetProperty("AreaSqft")?.SetValue(property, 1020);
            property.GetType().GetProperty("Lat")?.SetValue(property, 37.7749m);
            property.GetType().GetProperty("Lng")?.SetValue(property, -122.4194m);
            property.GetType().GetProperty("IsFeatured")?.SetValue(property, true);

            await context.Properties.AddAsync(property);

            var image = PropertyImage.Create(
                Guid.Parse("3f5b1f8f-6e2a-4d3a-8a2a-439bc8f76b11"),
                "https://cdn.example.com/properties/3f5b1f8f/main.jpg",
                StorageProvider.S3,
                "Front view",
                true,
                1
            );
            image.GetType().GetProperty("Id")?.SetValue(image, Guid.Parse("b2a81e6d-8a9e-4d7c-8b3f-120d4b47c9a1"));

            await context.PropertyImages.AddAsync(image);

            var trace = PropertyTrace.Create(
                Guid.Parse("3f5b1f8f-6e2a-4d3a-8a2a-439bc8f76b11"),
                TraceEventType.CREATED,
                "Initial creation",
                actorName: "system"
            );
            trace.GetType().GetProperty("Id")?.SetValue(trace, Guid.Parse("a8d0a587-2a60-4131-b98b-2a5e9a1f1d31"));

            await context.PropertyTraces.AddAsync(trace);

            await context.SaveChangesAsync();
        }
    }
}
