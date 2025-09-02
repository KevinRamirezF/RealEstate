using RealEstate.Application.Common.Interfaces;
using System.Reflection;

namespace RealEstate.Application.Commands.Properties;

public class DeletePropertyCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePropertyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(DeletePropertyCommand command, CancellationToken cancellationToken = default)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(command.Id, cancellationToken);
        if (property == null)
            return false;

        var propertyType = typeof(Domain.Entities.Property);
        var deletedAtProperty = propertyType.GetProperty(nameof(Domain.Entities.Property.DeletedAt), BindingFlags.Public | BindingFlags.Instance);
        if (deletedAtProperty != null && deletedAtProperty.CanWrite)
        {
            deletedAtProperty.SetValue(property, DateTime.UtcNow);
        }
        else
        {
            var backingField = propertyType.GetField($"<{nameof(Domain.Entities.Property.DeletedAt)}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            backingField?.SetValue(property, DateTime.UtcNow);
        }

        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}