using Ambev.DeveloperEvaluation.Application.Products.Query.GetById;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetById;

public class GetProductByIdProfile : Profile
{
    public GetProductByIdProfile()
    {
        CreateMap<GetProductByIdRequest, GetProductByIdQuery>();
    }
}
