namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetById;

public class GetSaleByIdRequest
{
    public Guid Id { get; set; }

    public GetSaleByIdRequest(Guid id)
    {
        Id = id;
    }
}
