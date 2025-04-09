using Ambev.DeveloperEvaluation.Application.Sales.Querys.GetAll;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetAll;

public class GetAllSalesProfile : Profile
{
    public GetAllSalesProfile()
    {
        CreateMap<GetAllSalesPaginationRequest, GetAllSalesQuery>();
    }
}
