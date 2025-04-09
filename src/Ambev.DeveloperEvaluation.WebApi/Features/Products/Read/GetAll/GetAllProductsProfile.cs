using Ambev.DeveloperEvaluation.Application.Products.Query.GetAll;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetAll;

public class GetAllProductsProfile : Profile
{
    public GetAllProductsProfile()
    {
        CreateMap<GetAllProductsPaginationRequest, GetAllProductsQuery>();
    }
}
