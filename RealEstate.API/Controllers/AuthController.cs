using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstate.API.Filters;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Input;
using RealEstate.Application.DTOs.Output;
using RealEstate.Domain.Identity;
using RealEstate.Infrastructure.Identity;

namespace RealEstate.API.Controllers;

/// <summary>
/// Authentication controller for user registration and login
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[ValidationFilter]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }


    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="loginDto">User login credentials</param>
    /// <returns>Authentication token if credentials are valid</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthTokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || user.DeletedAt.HasValue)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid credentials",
                Detail = "Email or password is incorrect.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid credentials",
                Detail = "Email or password is incorrect.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        // Generate token
        var roles = await _userManager.GetRolesAsync(user);
        var token = await _tokenService.GenerateTokenAsync(user, roles);
        var expiresAt = DateTimeOffset.UtcNow.AddHours(24);

        var authTokenDto = new AuthTokenDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty
            }
        };

        return Ok(authTokenDto);
    }

    /// <summary>
    /// Creates a new user account with specified role (Admin only) - ONLY way to create users
    /// </summary>
    /// <param name="createUserDto">User creation information including role</param>
    /// <returns>Authentication token for the newly created user</returns>
    /// <response code="201">User account created successfully</response>
    /// <response code="400">Invalid user data or user already exists</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions (Admin role required)</response>
    /// <remarks>
    /// **SECURITY**: This is the ONLY endpoint for creating users in the system.
    /// There is NO public registration endpoint for security reasons.
    /// 
    /// **Access**: Restricted to administrators only.
    /// **Capabilities**: Create users with any role (Admin or Guest).
    /// **Bootstrap**: Initial admin user is created automatically via database seeding.
    /// 
    /// **Usage Flow**:
    /// 1. System creates initial admin via seeding (admin@realestate.com / Admin123!)
    /// 2. Admin logs in and gets JWT token
    /// 3. Admin uses this endpoint to create additional users
    /// 
    /// **Security Model**: No public user creation = No unauthorized privilege escalation.
    /// </remarks>
    [HttpPost("create-user")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(AuthTokenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(createUserDto.Email);
        if (existingUser != null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "User already exists",
                Detail = "A user with this email address already exists.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(createUserDto.Role))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid role",
                Detail = $"Role '{createUserDto.Role}' does not exist.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        // Create user
        var user = new ApplicationUser
        {
            UserName = createUserDto.Email,
            Email = createUserDto.Email,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return BadRequest(new ProblemDetails
            {
                Title = "User creation failed",
                Detail = errors,
                Status = StatusCodes.Status400BadRequest
            });
        }

        // Add user to specified role
        await _userManager.AddToRoleAsync(user, createUserDto.Role);

        // Generate token
        var roles = await _userManager.GetRolesAsync(user);
        var token = await _tokenService.GenerateTokenAsync(user, roles);
        var expiresAt = DateTimeOffset.UtcNow.AddHours(24);

        var authTokenDto = new AuthTokenDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty
            }
        };

        return Created($"/api/v1/auth/users/{user.Id}", authTokenDto);
    }
}