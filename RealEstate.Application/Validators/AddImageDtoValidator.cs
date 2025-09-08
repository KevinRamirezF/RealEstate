using FluentValidation;
using RealEstate.Application.DTOs.Input;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Validators;

public class AddImageDtoValidator : AbstractValidator<AddImageDto>
{
    public AddImageDtoValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("URL is required.")
            .MaximumLength(1000)
            .WithMessage("URL cannot exceed 1000 characters.")
            .Must(BeValidUrl)
            .WithMessage("URL must be a valid HTTP or HTTPS URL.");

        RuleFor(x => x.StorageProvider)
            .NotEmpty()
            .WithMessage("Storage Provider is required.")
            .Must(BeValidStorageProvider)
            .WithMessage("Invalid Storage Provider. Valid values: S3, GCS, AZURE, LOCAL");

        RuleFor(x => x.AltText)
            .MaximumLength(200)
            .WithMessage("Alt Text cannot exceed 200 characters.");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo((short)0)
            .WithMessage("Sort Order must be 0 or greater.")
            .LessThanOrEqualTo((short)9999)
            .WithMessage("Sort Order cannot exceed 9999.");
    }

    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    private static bool BeValidStorageProvider(string storageProvider)
    {
        return Enum.TryParse<StorageProvider>(storageProvider, out _);
    }
}