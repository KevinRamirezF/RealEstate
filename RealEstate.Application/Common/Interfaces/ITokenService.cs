using RealEstate.Domain.Identity;

namespace RealEstate.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(IUser user, IList<string> roles);
}