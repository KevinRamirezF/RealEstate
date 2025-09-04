using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RealEstate.API.Filters;
using RealEstate.Application.Commands.Properties;
using RealEstate.Application.DTOs.Common;
using RealEstate.Application.DTOs.Filters;
using RealEstate.Application.DTOs.Input;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Queries.Properties;
using System.Text.Json;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[ValidationFilter]
public class PropertiesController : ControllerBase
{
    private readonly CreatePropertyCommandHandler _createPropertyHandler;
    private readonly GetPropertiesQueryHandler _getPropertiesHandler;
    private readonly GetPropertyByIdQueryHandler _getPropertyByIdHandler;
    private readonly UpdatePropertyCommandHandler _updatePropertyHandler;
    private readonly PatchPropertyCommandHandler _patchPropertyHandler;
    private readonly DeletePropertyCommandHandler _deletePropertyHandler;

    public PropertiesController(
        CreatePropertyCommandHandler createPropertyHandler,
        GetPropertiesQueryHandler getPropertiesHandler,
        GetPropertyByIdQueryHandler getPropertyByIdHandler,
        UpdatePropertyCommandHandler updatePropertyHandler,
        PatchPropertyCommandHandler patchPropertyHandler,
        DeletePropertyCommandHandler deletePropertyHandler)
    {
        _createPropertyHandler = createPropertyHandler;
        _getPropertiesHandler = getPropertiesHandler;
        _getPropertyByIdHandler = getPropertyByIdHandler;
        _updatePropertyHandler = updatePropertyHandler;
        _patchPropertyHandler = patchPropertyHandler;
        _deletePropertyHandler = deletePropertyHandler;
    }

    /// <summary>
    /// Retrieves a paginated list of properties with optional filters
    /// </summary>
    /// <param name="filters">Query parameters for filtering, sorting, and pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of properties</returns>
    [HttpGet]
    [OutputCache(Duration = 300, VaryByQueryKeys = ["page", "pageSize", "sort", "dir", "q", "ownerId", "minPrice", "maxPrice", "yearFrom", "yearTo", "propertyType", "listingStatus", "isFeatured", "isPublished", "state", "city", "postalCode"])]
    [ProducesResponseType<PagedResult<PropertyListDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<PropertyListDto>>> GetProperties(
        [FromQuery] PropertyFilters filters,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPropertiesQuery { Filters = filters };
        var result = await _getPropertiesHandler.HandleAsync(query, cancellationToken);
        
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
    /// Retrieves a property by its ID
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Property details</returns>
    [HttpGet("{id:guid}")]
    [OutputCache(Duration = 600, VaryByRouteValueNames = ["id"])]
    [ProducesResponseType<PropertyDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyDetailDto>> GetProperty(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPropertyByIdQuery { Id = id };
        var result = await _getPropertyByIdHandler.HandleAsync(query, cancellationToken);
        
        if (result == null)
            throw new KeyNotFoundException($"Property with ID '{id}' was not found.");
        
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
    /// Creates a new property
    /// </summary>
    /// <param name="createPropertyDto">Property creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created property</returns>
    [HttpPost]
    [ProducesResponseType<PropertyDetailDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropertyDetailDto>> CreateProperty(
        [FromBody] CreatePropertyDto createPropertyDto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreatePropertyCommand { Data = createPropertyDto };
        var result = await _createPropertyHandler.HandleAsync(command, cancellationToken);
        
        return CreatedAtAction(
            nameof(GetProperty),
            new { id = result.Id },
            result);
    }

    /// <summary>
    /// Updates an existing property
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="updatePropertyDto">Property update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated property</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<PropertyDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropertyDetailDto>> UpdateProperty(
        Guid id,
        [FromBody] UpdatePropertyDto updatePropertyDto,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdatePropertyCommand { Id = id, Data = updatePropertyDto };
        var result = await _updatePropertyHandler.HandleAsync(command, cancellationToken);
        
        if (result == null)
            throw new KeyNotFoundException($"Property with ID '{id}' was not found.");
        
        return Ok(result);
    }

    /// <summary>
    /// Partially updates an existing property
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="patchPropertyDto">Property partial update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated property</returns>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType<PropertyDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropertyDetailDto>> PatchProperty(
        Guid id,
        [FromBody] PatchPropertyDto patchPropertyDto,
        CancellationToken cancellationToken = default)
    {
        var command = new PatchPropertyCommand { Id = id, Data = patchPropertyDto };
        var result = await _patchPropertyHandler.HandleAsync(command, cancellationToken);
        
        if (result == null)
            throw new KeyNotFoundException($"Property with ID '{id}' was not found.");
        
        return Ok(result);
    }

    /// <summary>
    /// Soft deletes a property
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProperty(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeletePropertyCommand { Id = id };
        var success = await _deletePropertyHandler.HandleAsync(command, cancellationToken);
        
        if (!success)
            throw new KeyNotFoundException($"Property with ID '{id}' was not found.");
        
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