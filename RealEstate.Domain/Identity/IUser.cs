namespace RealEstate.Domain.Identity;

public interface IUser
{
    Guid Id { get; }
    string UserName { get; }
    string Email { get; }
    string FirstName { get; }
    string LastName { get; }
    string FullName { get; }
}