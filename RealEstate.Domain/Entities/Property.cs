using System;
using System.Collections.Generic;
using System.Linq;

namespace RealEstate.Domain.Entities;

public class Property
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public decimal Price { get; private set; }
    public string CodeInternal { get; private set; }
    public int Year { get; private set; }
    public Guid OwnerId { get; private set; }

    private readonly List<PropertyImage> _images = new();
    public IReadOnlyCollection<PropertyImage> Images => _images.AsReadOnly();

    private readonly List<PropertyTrace> _traces = new();
    public IReadOnlyCollection<PropertyTrace> Traces => _traces.AsReadOnly();

    private Property(Guid id, string name, string address, decimal price, string codeInternal, int year, Guid ownerId)
    {
        Id = id;
        Name = name;
        Address = address;
        Price = price;
        CodeInternal = codeInternal;
        Year = year;
        OwnerId = ownerId;
    }

    public static Property Create(string name, string address, decimal price, string codeInternal, int year, Guid ownerId)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));
        if (string.IsNullOrWhiteSpace(codeInternal))
            throw new ArgumentException("CodeInternal is required.", nameof(codeInternal));

        return new Property(Guid.NewGuid(), name, address, price, codeInternal, year, ownerId);
    }

    public void ChangePrice(decimal newPrice, decimal tax = 0)
    {
        if (newPrice < 0)
            throw new ArgumentException("New price cannot be negative.", nameof(newPrice));

        var trace = PropertyTrace.Create(Id, "Price Change", newPrice, tax);
        _traces.Add(trace);

        Price = newPrice;
    }

    public void AddImage(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl) || !Uri.IsWellFormedUriString(fileUrl, UriKind.Absolute))
            throw new ArgumentException("Invalid file URL.", nameof(fileUrl));

        bool isFirstImage = !_images.Any();
        var image = PropertyImage.Create(Id, fileUrl, isFirstImage);
        _images.Add(image);
    }
}
