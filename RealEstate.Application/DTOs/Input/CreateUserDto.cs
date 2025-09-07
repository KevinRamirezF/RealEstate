namespace RealEstate.Application.DTOs.Input;

/// <summary>
/// DTO for admin-only user creation
/// </summary>
public record CreateUserDto
{
    /// <summary>
    /// User's first name
    /// </summary>
    /// <example>John</example>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    /// <example>Doe</example>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    /// <example>MySecurePassword123</example>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Password confirmation
    /// </summary>
    /// <example>MySecurePassword123</example>
    public string ConfirmPassword { get; init; } = string.Empty;

    /// <summary>
    /// User role (Admin or Guest) - Only admins can set this
    /// </summary>
    /// <example>Guest</example>
    public string Role { get; init; } = string.Empty;
}