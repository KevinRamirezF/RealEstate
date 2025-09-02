using RealEstate.Application.DTOs.Input;
using RealEstate.Application.DTOs.Output;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Mappers;

public class PropertyMapper
{
    // Manual mappings since entities use factory methods and private constructors
    public PropertyListDto ToListDto(Property entity, string ownerFullName)
    {
        return new PropertyListDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CodeInternal = entity.CodeInternal,
            City = entity.City,
            State = entity.State,
            PostalCode = entity.PostalCode,
            Price = entity.Price,
            YearBuilt = entity.YearBuilt,
            Bedrooms = entity.Bedrooms,
            Bathrooms = entity.Bathrooms,
            AreaSqft = entity.AreaSqft,
            ListingStatus = entity.ListingStatus.ToString(),
            PrimaryImageUrl = entity.Images.FirstOrDefault(i => i.IsPrimary)?.Url,
            OwnerFullName = ownerFullName,
            IsFeatured = entity.IsFeatured,
            IsPublished = entity.IsPublished
        };
    }

    public PropertyDetailDto ToDetailDto(Property entity, string ownerFullName, PropertyTrace? lastPriceChange = null)
    {
        return new PropertyDetailDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CodeInternal = entity.CodeInternal,
            Description = entity.Description,
            PropertyType = entity.PropertyType.ToString(),
            YearBuilt = entity.YearBuilt,
            Bedrooms = entity.Bedrooms,
            Bathrooms = entity.Bathrooms,
            ParkingSpaces = entity.ParkingSpaces,
            AreaSqft = entity.AreaSqft,
            LotSizeSqft = entity.LotSizeSqft,
            Price = entity.Price,
            Currency = entity.Currency,
            HoaFee = entity.HoaFee,
            AddressLine1 = entity.AddressLine1,
            AddressLine2 = entity.AddressLine2,
            City = entity.City,
            State = entity.State,
            PostalCode = entity.PostalCode,
            Country = entity.Country,
            Lat = entity.Lat,
            Lng = entity.Lng,
            ListingStatus = entity.ListingStatus.ToString(),
            ListingDate = entity.ListingDate,
            LastSoldPrice = entity.LastSoldPrice,
            IsFeatured = entity.IsFeatured,
            IsPublished = entity.IsPublished,
            OwnerFullName = ownerFullName,
            Images = entity.Images.OrderBy(i => !i.IsPrimary).ThenBy(i => i.SortOrder).Select(ToImageDto).ToList(),
            LastPriceChange = ToLastPriceChangeDto(lastPriceChange),
            TracesCount = entity.Traces.Count,
            RowVersion = entity.RowVersion
        };
    }

    public PropertyImageDto ToImageDto(PropertyImage entity)
    {
        return new PropertyImageDto
        {
            Id = entity.Id,
            Url = entity.Url,
            IsPrimary = entity.IsPrimary,
            SortOrder = entity.SortOrder,
            Enabled = entity.Enabled,
            AltText = entity.AltText
        };
    }

    public PropertyPriceChangeDto? ToLastPriceChangeDto(PropertyTrace? trace)
    {
        if (trace == null || trace.EventType != TraceEventType.PRICE_CHANGE)
            return null;

        return new PropertyPriceChangeDto
        {
            EventDate = trace.EventDate,
            OldValue = trace.OldValue,
            NewValue = trace.NewValue,
            TaxAmount = trace.TaxAmount,
            ActorName = trace.ActorName
        };
    }
}