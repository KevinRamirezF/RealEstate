using RealEstate.Application.Common.Interfaces;
using System.Reflection;

namespace RealEstate.Application.Commands.Owners;

public class DeleteOwnerCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOwnerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(DeleteOwnerCommand command, CancellationToken cancellationToken = default)
    {
        var owner = await _unitOfWork.Owners.GetByIdAsync(command.Id, cancellationToken);
        if (owner == null)
            return false;

        var ownerType = typeof(Domain.Entities.Owner);
        var deletedAtProperty = ownerType.GetProperty(nameof(Domain.Entities.Owner.DeletedAt), BindingFlags.Public | BindingFlags.Instance);
        if (deletedAtProperty != null && deletedAtProperty.CanWrite)
        {
            deletedAtProperty.SetValue(owner, DateTime.UtcNow);
        }
        else
        {
            var backingField = ownerType.GetField($"<{nameof(Domain.Entities.Owner.DeletedAt)}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            backingField?.SetValue(owner, DateTime.UtcNow);
        }

        _unitOfWork.Owners.Update(owner);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}