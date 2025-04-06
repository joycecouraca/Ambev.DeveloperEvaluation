namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetAll;

/// <summary>
/// Represents a request for paginated product retrieval.
/// Contains pagination parameters and an optional search category.
/// </summary>
public class GetAllProductsPaginationRequest
{
    /// <summary>
    /// Gets or sets the page number.
    /// Must be greater than 0.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the page size.
    /// Must be greater than 0 and less than or equal to 100.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the search category.
    /// Optional. Must be 30 characters or less.
    /// </summary>
    public string? Search { get; set; }
}

