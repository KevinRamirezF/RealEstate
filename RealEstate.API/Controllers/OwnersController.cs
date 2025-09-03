using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RealEstate.API.Filters;
using RealEstate.Application.Commands.Owners;
using RealEstate.Application.DTOs.Input;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Queries.Owners;
using System.Text.Json;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[ValidationFilter]
public class OwnersController : ControllerBase
{
    private readonly CreateOwnerCommandHandler _createOwnerHandler;
    private readonly GetOwnersQueryHandler _getOwnersHandler;
    private readonly GetOwnerByIdQueryHandler _getOwnerByIdHandler;
    private readonly UpdateOwnerCommandHandler _updateOwnerHandler;
    private readonly DeleteOwnerCommandHandler _deleteOwnerHandler;

    public OwnersController(
        CreateOwnerCommandHandler createOwnerHandler,
        GetOwnersQueryHandler getOwnersHandler,
        GetOwnerByIdQueryHandler getOwnerByIdHandler,
        UpdateOwnerCommandHandler updateOwnerHandler,
        DeleteOwnerCommandHandler deleteOwnerHandler)
    {
        _createOwnerHandler = createOwnerHandler;
        _getOwnersHandler = getOwnersHandler;
        _getOwnerByIdHandler = getOwnerByIdHandler;
        _updateOwnerHandler = updateOwnerHandler;
        _deleteOwnerHandler = deleteOwnerHandler;
    }

    /// <summary>
    /// Retrieves all owners
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of owners</returns>
    [HttpGet]
    [OutputCache(Duration = 300)]
    [ProducesResponseType<IEnumerable<OwnerListDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OwnerListDto>>> GetOwners(
        CancellationToken cancellationToken = default)
    {
        var query = new GetOwnersQuery();
        var result = await _getOwnersHandler.HandleAsync(query, cancellationToken);
        
        // Generate ETag based on result content
        var etag = GenerateETag(result);
        Response.Headers.ETag = etag;
        
        // Check If-None-Match header for conditional requests
        if (Request.Headers.IfNoneMatch.Contains(etag))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Retrieves an owner by ID
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Owner details</returns>
    [HttpGet("{id:guid}")]
    [OutputCache(Duration = 600, VaryByRouteValueNames = ["id"])]
    [ProducesResponseType<OwnerDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OwnerDetailDto>> GetOwner(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOwnerByIdQuery { Id = id };
        var result = await _getOwnerByIdHandler.HandleAsync(query, cancellationToken);
        
        if (result == null)
            throw new KeyNotFoundException($"Owner with ID '{id}' was not found.");
        
        // Generate ETag based on result content
        var etag = GenerateETag(result);
        Response.Headers.ETag = etag;
        
        // Check If-None-Match header for conditional requests
        if (Request.Headers.IfNoneMatch.Contains(etag))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Creates a new owner
    /// </summary>
    /// <param name="createOwnerDto">Owner creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created owner</returns>
    [HttpPost]
    [ProducesResponseType<OwnerDetailDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OwnerDetailDto>> CreateOwner(
        [FromBody] CreateOwnerDto createOwnerDto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateOwnerCommand { Data = createOwnerDto };
        var result = await _createOwnerHandler.HandleAsync(command, cancellationToken);
        
        return CreatedAtAction(
            nameof(GetOwner),
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Updates an existing owner
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <param name="updateOwnerDto">Owner update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated owner</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<OwnerDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OwnerDetailDto>> UpdateOwner(
        Guid id,
        [FromBody] UpdateOwnerDto updateOwnerDto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateOwnerCommand { Id = id, Data = updateOwnerDto };
        var result = await _updateOwnerHandler.HandleAsync(command, cancellationToken);
        
        if (result == null)
            throw new KeyNotFoundException($"Owner with ID '{id}' was not found.");
        
        return Ok(result);
    }

    /// <summary>
    /// Soft deletes an owner
    /// </summary>
    /// <param name="id">Owner ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteOwner(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteOwnerCommand { Id = id };
        var success = await _deleteOwnerHandler.HandleAsync(command, cancellationToken);
        
        if (!success)
            throw new KeyNotFoundException($"Owner with ID '{id}' was not found.");
        
        return NoContent();
    }

    private string GenerateETag(object data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json));
        return $"\"{Convert.ToBase64String(hash)[..8]}\"";
    }
}