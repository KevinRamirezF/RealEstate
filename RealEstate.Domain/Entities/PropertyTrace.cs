using System;

namespace RealEstate.Domain.Entities;

public class PropertyTrace
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public DateTime DateSale { get; private set; }
    public string Name { get; private set; }
    public decimal Value { get; private set; }
    public decimal Tax { get; private set; }

    private PropertyTrace(Guid id, Guid propertyId, DateTime dateSale, string name, decimal value, decimal tax)
    {
        Id = id;
        PropertyId = propertyId;
        DateSale = dateSale;
        Name = name;
        Value = value;
        Tax = tax;
    }

    internal static PropertyTrace Create(Guid propertyId, string name, decimal value, decimal tax)
    {
        return new PropertyTrace(Guid.NewGuid(), propertyId, DateTime.UtcNow, name, value, tax);
    }
}
