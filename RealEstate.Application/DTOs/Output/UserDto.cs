namespace RealEstate.Application.DTOs.Output;

/// <summary>
/// DTO containing user information
/// </summary>
public record UserDto
{
    /// <summary>
    /// User's unique identifier
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid Id { get; init; }

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
    /// User's full name
    /// </summary>
    /// <example>John Doe</example>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's role
    /// </summary>
    /// <example>Admin</example>
    public string Role { get; init; } = string.Empty;
}