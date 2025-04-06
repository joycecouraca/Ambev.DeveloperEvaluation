namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Read;

public class GetAllProductsPaginationRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchCategory { get; set; }
}
