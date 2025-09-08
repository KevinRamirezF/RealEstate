namespace RealEstate.Application.DTOs.Input;

/// <summary>
/// DTO for user login
/// </summary>
public record LoginDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    /// <example>admin@realestate.com</example>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    /// <example>Admin123!</example>
    public string Password { get; init; } = string.Empty;
}