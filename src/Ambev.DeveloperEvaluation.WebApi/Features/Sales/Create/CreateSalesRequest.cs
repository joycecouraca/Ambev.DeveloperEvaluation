namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;

public class CreateSalesRequest
{
    public DateTime SoldAt { get; set; }
    public string BranchName { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public List<SaleItemRequest> Items { get; set; } = new();
}

public class SaleItemRequest
{
    /// <summary>
    /// Identificador do produto.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Quantidade do produto.
    /// </summary>
    public int Quantity { get; set; }
}
