using System;

namespace RealEstate.Domain.Entities;

public class Owner
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public string Photo { get; private set; }
    public DateTime Birthday { get; private set; }

    private Owner(Guid id, string name, string address, string photo, DateTime birthday)
    {
        Id = id;
        Name = name;
        Address = address;
        Photo = photo;
        Birthday = birthday;
    }

    public static Owner Create(string name, string address, string photo, DateTime birthday)
    {
        // Validations can be added here
        return new Owner(Guid.NewGuid(), name, address, photo, birthday);
    }
}
