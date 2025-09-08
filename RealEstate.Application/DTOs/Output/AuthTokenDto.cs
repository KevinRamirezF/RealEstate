namespace RealEstate.Application.DTOs.Output;

/// <summary>
/// DTO containing authentication token information
/// </summary>
public record AuthTokenDto
{
    /// <summary>
    /// JWT access token
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// Token expiration date and time
    /// </summary>
    /// <example>2024-09-07T10:30:00Z</example>
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; init; } = new();
}