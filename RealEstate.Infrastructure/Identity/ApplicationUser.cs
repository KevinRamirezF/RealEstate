using Microsoft.AspNetCore.Identity;
using RealEstate.Domain.Identity;
using System;

namespace RealEstate.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>, IUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DeletedAt { get; set; }
        
        public string FullName => $"{FirstName} {LastName}".Trim();

        // Explicit interface implementation to handle nullable string properties
        string IUser.UserName => UserName ?? string.Empty;
        string IUser.Email => Email ?? string.Empty;
        string IUser.FirstName => FirstName ?? string.Empty;
        string IUser.LastName => LastName ?? string.Empty;
    }
}
