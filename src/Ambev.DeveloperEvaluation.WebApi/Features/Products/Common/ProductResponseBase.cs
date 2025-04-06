namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Common;

/// <summary>
/// Base class for product response containing common properties.
/// </summary>
public abstract class ProductResponseBase
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the category of the product.
    /// </summary>
    public string Category { get; set; } = default!;
}
