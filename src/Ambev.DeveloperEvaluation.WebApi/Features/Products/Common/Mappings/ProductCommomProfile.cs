using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Common.Response;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Common.Mappings;

public class ProductCommomProfile : Profile
{
    public ProductCommomProfile()
    {
        CreateMap<ProductDto, ProductResponse>()
               .ForMember(dest => dest.Category, src => src.MapFrom(s => s.Category.ToString()));
    }
}
