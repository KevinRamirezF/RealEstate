using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Validators;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Commands.Properties;

public class AddImageCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddImageDtoValidator _validator;

    public AddImageCommandHandler(IUnitOfWork unitOfWork, AddImageDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<AddImageResult> Handle(AddImageCommand command, CancellationToken cancellationToken = default)
    {
        // Validate input
        var validationResult = await _validator.ValidateAsync(command.Image, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Get existing property with images
        var property = await _unitOfWork.Properties.GetPropertyWithImagesAndTracesAsync(command.PropertyId, cancellationToken);
        if (property == null)
        {
            throw new ArgumentException($"Property with ID {command.PropertyId} not found.");
        }

        // Parse storage provider
        var storageProvider = Enum.Parse<StorageProvider>(command.Image.StorageProvider);

        // Add image using domain method (handles primary image logic)
        property.AddImage(
            command.Image.Url,
            storageProvider,
            command.Image.AltText,
            command.Image.IsPrimary,
            command.Image.SortOrder
        );

        // Update in repository
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Get the newly added image
        var newImage = property.Images.OrderByDescending(i => i.CreatedAt).First();

        return new AddImageResult
        {
            Id = newImage.Id,
            PropertyId = newImage.PropertyId,
            Url = newImage.Url,
            StorageProvider = newImage.StorageProvider.ToString(),
            AltText = newImage.AltText,
            IsPrimary = newImage.IsPrimary,
            SortOrder = newImage.SortOrder,
            CreatedAt = newImage.CreatedAt
        };
    }
}