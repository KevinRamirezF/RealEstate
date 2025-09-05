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
    private readonly ChangePriceCommandHandler _changePriceHandler;
    private readonly AddImageCommandHandler _addImageHandler;

    public PropertiesController(
        CreatePropertyCommandHandler createPropertyHandler,
        GetPropertiesQueryHandler getPropertiesHandler,
        GetPropertyByIdQueryHandler getPropertyByIdHandler,
        UpdatePropertyCommandHandler updatePropertyHandler,
        PatchPropertyCommandHandler patchPropertyHandler,
        DeletePropertyCommandHandler deletePropertyHandler,
        ChangePriceCommandHandler changePriceHandler,
        AddImageCommandHandler addImageHandler)
    {
        _createPropertyHandler = createPropertyHandler;
        _getPropertiesHandler = getPropertiesHandler;
        _getPropertyByIdHandler = getPropertyByIdHandler;
        _updatePropertyHandler = updatePropertyHandler;
        _patchPropertyHandler = patchPropertyHandler;
        _deletePropertyHandler = deletePropertyHandler;
        _changePriceHandler = changePriceHandler;
        _addImageHandler = addImageHandler;
    }

    /// <summary>
    /// Retrieves a paginated list of properties with optional filters
    /// </summary>
    /// <param name="filters">Query parameters for filtering, sorting, and pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of properties</returns>
    /// <remarks>
    /// Sample query parameters:
    /// 
    ///     GET /api/v1/properties?page=1&amp;pageSize=10&amp;sort=Price&amp;dir=desc&amp;propertyType=HOUSE&amp;minPrice=100000&amp;maxPrice=500000&amp;state=CA&amp;isFeatured=true
    /// 
    /// Available query parameters:
    /// - **page**: Page number (default: 1)
    /// - **pageSize**: Items per page (1-100, default: 20)
    /// - **sort**: Sort field (Name, Price, CreatedAt, ListingDate)
    /// - **dir**: Sort direction (asc, desc)
    /// - **q**: Search query (searches in name and description)
    /// - **ownerId**: Filter by owner ID
    /// - **minPrice**, **maxPrice**: Price range filters
    /// - **yearFrom**, **yearTo**: Year built range filters
    /// - **propertyType**: HOUSE, APARTMENT, CONDO, TOWNHOUSE, LAND, COMMERCIAL, OTHER
    /// - **listingStatus**: ACTIVE, INACTIVE, SOLD, RENTED, PENDING
    /// - **isFeatured**: true/false
    /// - **isPublished**: true/false
    /// - **state**: State/province filter
    /// - **city**: City filter
    /// - **postalCode**: Postal code filter
    /// </remarks>
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
    /// Creates a new property listing
    /// </summary>
    /// <param name="createPropertyDto">Property creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created property</returns>
    /// <remarks>
    /// Sample request body:
    /// 
    ///     POST /api/v1/properties
    ///     {
    ///         "ownerId": "16cbcd24-2318-4ba3-aa8a-dfaafc077a39",
    ///         "codeInternal": "PROP001",
    ///         "name": "Beautiful Family Home",
    ///         "description": "A spacious 3-bedroom home with modern amenities, updated kitchen, and large backyard perfect for families.",
    ///         "propertyType": "HOUSE",
    ///         "yearBuilt": 2020,
    ///         "bedrooms": 3,
    ///         "bathrooms": 2,
    ///         "parkingSpaces": 2,
    ///         "areaSqft": 2500,
    ///         "price": 450000.00,
    ///         "currency": "USD",
    ///         "addressLine": "123 Main Street",
    ///         "city": "Los Angeles",
    ///         "state": "CA",
    ///         "postalCode": "90210",
    ///         "country": "US",
    ///         "lat": 34.0522,
    ///         "lng": -118.2437,
    ///         "listingStatus": "ACTIVE",
    ///         "listingDate": "2024-01-15",
    ///         "isFeatured": false,
    ///         "isPublished": true
    ///     }
    /// 
    /// **Required fields**: ownerId, codeInternal, name, propertyType, addressLine, city, state, postalCode
    /// 
    /// **Property Types**: HOUSE, APARTMENT, CONDO, TOWNHOUSE, LAND, COMMERCIAL, OTHER
    /// 
    /// **Listing Status**: ACTIVE, INACTIVE, SOLD, RENTED, PENDING
    /// 
    /// **Currency Codes**: USD, EUR, GBP, CAD, MXN (3-character ISO codes)
    /// 
    /// **Country Codes**: US, CA, MX (2-character ISO codes)
    /// </remarks>
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
    /// Partially updates an existing property (PATCH - only send fields you want to update)
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="patchPropertyDto">Property partial update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated property</returns>
    /// <remarks>
    /// Sample request body (update only specific fields):
    /// 
    ///     PATCH /api/v1/properties/cb1a3a83-255b-4e41-97ab-c1197469bd9a
    ///     {
    ///         "name": "Updated Property Name",
    ///         "price": 475000.00,
    ///         "listingStatus": "SOLD",
    ///         "isFeatured": true,
    ///         "rowVersion": 1
    ///     }
    /// 
    /// **Key Features**:
    /// - Send only the fields you want to update (all fields are optional)
    /// - Omit fields you don't want to change
    /// - Include rowVersion for optimistic concurrency control
    /// 
    /// **Common use cases**:
    /// - Update just the price: `{"price": 500000, "rowVersion": 1}`
    /// - Change status only: `{"listingStatus": "SOLD", "rowVersion": 1}`
    /// - Update location: `{"city": "San Francisco", "state": "CA", "rowVersion": 1}`
    /// - Mark as featured: `{"isFeatured": true, "rowVersion": 1}`
    /// 
    /// **Available fields**: name, description, yearBuilt, bedrooms, bathrooms, parkingSpaces, areaSqft, 
    /// addressLine, city, state, postalCode, lat, lng, listingStatus, listingDate, isFeatured, isPublished, rowVersion
    /// </remarks>
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
    /// Changes the price of a property
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="basePrice">Base price value (required)</param>
    /// <param name="taxAmount">Tax amount for the property (required)</param>
    /// <param name="actorName">Name of person making the change (optional)</param>
    /// <param name="rowVersion">Row version for concurrency control (required)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Price change result with old/new prices and audit information</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/v1/properties/3fa85f64-5717-4562-b3fc-2c963f66afa6/change-price?basePrice=2500000.00&amp;taxAmount=250000.00&amp;actorName=Kevin%20Ramirez&amp;rowVersion=1
    ///     
    /// This endpoint creates a price change trace for audit purposes and updates the property price atomically.
    /// </remarks>
    [HttpPut("{id:guid}/change-price")]
    [ProducesResponseType<ChangePriceResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ChangePriceResult>> ChangePrice(
        Guid id,
        [FromQuery] decimal basePrice,
        [FromQuery] decimal taxAmount,
        [FromQuery] string? actorName = null,
        [FromQuery] int rowVersion = 1,
        CancellationToken cancellationToken = default)
    {
        var priceChangeDto = new ChangePriceDto
        {
            BasePrice = basePrice,
            TaxAmount = taxAmount,
            ActorName = actorName,
            RowVersion = rowVersion
        };

        var command = new ChangePriceCommand 
        { 
            Id = id, 
            PriceChange = priceChangeDto 
        };
        
        var result = await _changePriceHandler.Handle(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Adds an image to a property
    /// </summary>
    /// <param name="id">Property ID</param>
    /// <param name="imageDto">Image information to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Added image information with generated ID and creation timestamp</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/properties/3fa85f64-5717-4562-b3fc-2c963f66afa6/images
    ///     {
    ///       "url": "https://images.millionluxury.com/property-123/living-room.jpg",
    ///       "storageProvider": "S3",
    ///       "altText": "Spacious living room with ocean view",
    ///       "isPrimary": false,
    ///       "sortOrder": 2
    ///     }
    ///     
    /// Only one primary image is allowed per property. Setting isPrimary=true will make this the new primary image.
    /// </remarks>
    [HttpPost("{id:guid}/images")]
    [ProducesResponseType<AddImageResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddImageResult>> AddImage(
        Guid id,
        [FromBody] AddImageDto imageDto,
        CancellationToken cancellationToken = default)
    {
        var command = new AddImageCommand 
        { 
            PropertyId = id, 
            Image = imageDto 
        };
        
        var result = await _addImageHandler.Handle(command, cancellationToken);
        return CreatedAtAction(nameof(GetProperty), new { id = id }, result);
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