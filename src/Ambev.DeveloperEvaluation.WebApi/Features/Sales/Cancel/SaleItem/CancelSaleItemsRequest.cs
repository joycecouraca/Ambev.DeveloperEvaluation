namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.SaleItem;

public class CancelSaleItemsRequest
{
    public Guid SaleId { get; set; }
    public List<Guid> ItemIds { get; set; } = [];
    public CancelSaleItemsRequest(Guid saleId, List<Guid> itemIds)
    {
        SaleId = saleId;
        ItemIds = itemIds;
    }
}
