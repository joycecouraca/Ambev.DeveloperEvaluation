namespace Ambev.DeveloperEvaluation.Application.Products.Common;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Category { get; set; } = default!;
    public decimal Price { get; set; }
    public string Description { get; set; } = default!;
    public int Quantity { get; set; }
}
