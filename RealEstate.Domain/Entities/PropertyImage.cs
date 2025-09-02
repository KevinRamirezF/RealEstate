using System;

namespace RealEstate.Domain.Entities;

public class PropertyImage
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public string File { get; private set; }
    public bool Enabled { get; private set; }

    private PropertyImage(Guid id, Guid propertyId, string file, bool enabled)
    {
        Id = id;
        PropertyId = propertyId;
        File = file;
        Enabled = enabled;
    }

    internal static PropertyImage Create(Guid propertyId, string file, bool enabled)
    {
        return new PropertyImage(Guid.NewGuid(), propertyId, file, enabled);
    }

    internal void SetEnabled(bool enabled)
    {
        Enabled = enabled;
    }
}
