using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Infrastructure.Data;

public class DataSeeder
{
    private readonly RealEstateDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(RealEstateDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();

            if (!await _context.Owners.AnyAsync())
            {
                await SeedOwnersAsync();
            }

            if (!await _context.Properties.AnyAsync())
            {
                await SeedPropertiesAsync();
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Data seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding data");
            throw;
        }
    }

    private async Task SeedOwnersAsync()
    {
        var owners = new[]
        {
            Owner.Create(
                fullName: "María García López", 
                email: "maria.garcia@email.com",
                phone: "+1-555-0101"
            ),
            Owner.Create(
                fullName: "Carlos Rodríguez Martínez",
                email: "carlos.rodriguez@email.com", 
                phone: "+1-555-0102"
            )
        };

        await _context.Owners.AddRangeAsync(owners);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} owners", owners.Length);
    }

    private async Task SeedPropertiesAsync()
    {
        var owners = await _context.Owners.ToListAsync();
        if (!owners.Any())
        {
            _logger.LogWarning("No owners found for property seeding");
            return;
        }

        var properties = new[]
        {
            CreatePropertyWithImages(
                owners[0].Id,
                "CASA001",
                "Casa Moderna en Zona Residencial",
                PropertyType.HOUSE,
                450000m,
                "123 Maple Street",
                "Springfield",
                "Illinois",
                "62701",
                new[]
                {
                    "https://images.unsplash.com/photo-1568605114967-8130f3a36994?w=800&h=600&fit=crop",
                    "https://images.unsplash.com/photo-1570129477492-45c003edd2be?w=800&h=600&fit=crop",
                    "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800&h=600&fit=crop"
                }
            ),
            CreatePropertyWithImages(
                owners[1].Id,
                "APTO001", 
                "Apartamento de Lujo con Vista al Mar",
                PropertyType.APARTMENT,
                680000m,
                "456 Ocean Drive, Unit 12A",
                "Miami",
                "Florida", 
                "33139",
                new[]
                {
                    "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=800&h=600&fit=crop",
                    "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?w=800&h=600&fit=crop"
                }
            ),
            CreatePropertyWithImages(
                owners[0].Id,
                "COND001",
                "Condominio Ejecutivo en Centro de Negocios",
                PropertyType.CONDO,
                320000m,
                "789 Business Plaza, Suite 501",
                "Chicago",
                "Illinois",
                "60601",
                new[]
                {
                    "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800&h=600&fit=crop",
                    "https://images.unsplash.com/photo-1524813686514-a57563d77965?w=800&h=600&fit=crop",
                    "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=800&h=600&fit=crop",
                    "https://images.unsplash.com/photo-1505142468610-359e7d316be0?w=800&h=600&fit=crop"
                }
            )
        };

        await _context.Properties.AddRangeAsync(properties);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} properties with images", properties.Length);
    }

    private static Property CreatePropertyWithImages(Guid ownerId, string codeInternal, string name, 
        PropertyType propertyType, decimal price, string addressLine, string city, string state, 
        string postalCode, string[] imageUrls)
    {
        var property = Property.Create(ownerId, codeInternal, name, propertyType, price, 
            addressLine, city, state, postalCode);

        // Agregar detalles adicionales según el tipo de propiedad
        switch (propertyType)
        {
            case PropertyType.HOUSE:
                property.Update(
                    bedrooms: 4,
                    bathrooms: 4,
                    parkingSpaces: 2,
                    areaSqft: 2800
                );
                break;
            case PropertyType.APARTMENT:
                property.Update(
                    bedrooms: 3,
                    bathrooms: 2,
                    parkingSpaces: 1,
                    areaSqft: 1850
                );
                break;
            case PropertyType.CONDO:
                property.Update(
                    bedrooms: 2,
                    bathrooms: 2,
                    parkingSpaces: 1,
                    areaSqft: 1200
                );
                break;
        }

        // Agregar imágenes con enlaces externos
        for (int i = 0; i < imageUrls.Length; i++)
        {
            property.AddImage(
                url: imageUrls[i],
                storageProvider: StorageProvider.EXTERNAL,
                altText: $"{name} - Imagen {i + 1}",
                isPrimary: i == 0, // La primera imagen es primaria
                sortOrder: (short)i
            );
        }

        return property;
    }
}