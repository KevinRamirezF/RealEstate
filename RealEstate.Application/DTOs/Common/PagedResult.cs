using System.ComponentModel;

namespace RealEstate.Application.DTOs.Common;

/// <summary>
/// Generic paginated result container for API responses
/// </summary>
/// <typeparam name="T">Type of items being paginated</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// List of items for the current page
    /// </summary>
    [Description("List of items for the current page")]
    public List<T> Items { get; set; } = new();
    
    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    /// <example>150</example>
    [Description("Total number of items across all pages")]
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    /// <example>2</example>
    [Description("Current page number (1-based)")]
    public int Page { get; set; }
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    /// <example>20</example>
    [Description("Number of items per page")]
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total number of pages available
    /// </summary>
    /// <example>8</example>
    [Description("Total number of pages available")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    /// <summary>
    /// Whether there is a next page available
    /// </summary>
    /// <example>true</example>
    [Description("Whether there is a next page available")]
    public bool HasNextPage => Page < TotalPages;
    
    /// <summary>
    /// Whether there is a previous page available
    /// </summary>
    /// <example>true</example>
    [Description("Whether there is a previous page available")]
    public bool HasPreviousPage => Page > 1;
}