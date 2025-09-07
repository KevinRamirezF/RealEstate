namespace RealEstate.Domain.Identity;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Guest = "Guest";

    public static readonly string[] AllRoles = { Admin, Guest };
}