using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.SaleItem;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.SaleItem;

public class CancelSaleItemProfile : Profile
{
    public CancelSaleItemProfile()
    {
        CreateMap<CancelSaleItemsRequest, CancelSaleItemsCommand>();
    }
}
