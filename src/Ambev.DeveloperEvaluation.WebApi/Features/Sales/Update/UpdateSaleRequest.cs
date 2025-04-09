using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Update;

public class UpdateSaleRequest : CreateSalesRequest
{
    public Guid SaleId { get; set; }
}
