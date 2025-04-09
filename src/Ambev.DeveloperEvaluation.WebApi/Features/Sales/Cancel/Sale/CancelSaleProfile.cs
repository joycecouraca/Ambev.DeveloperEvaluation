using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Sale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.Sale;

public class CancelSaleProfile:Profile
{
    public CancelSaleProfile()
    {
        CreateMap<CancelSaleRequest, CancelSaleCommand>();
    }
}
