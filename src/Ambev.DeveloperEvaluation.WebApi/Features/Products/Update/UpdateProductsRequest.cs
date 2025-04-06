using Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Update;

public class UpdateProductsRequest : CreateProductsRequest
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public Guid Id { get; set; }
}
