using Ambev.DeveloperEvaluation.Application.Products.Query.GetAll;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetAll;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Read.GetAll;

public class GetAllProductsProfileTests
{
    private readonly IMapper _mapper;

    public GetAllProductsProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<GetAllProductsProfile>());

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_GetAllProductsPaginationRequest_To_GetAllProductsQuery()
    {
        var request = new GetAllProductsPaginationRequest
        {
            Page = 2,
            PageSize = 25,
            Search = "bebidas"
        };

        var result = _mapper.Map<GetAllProductsQuery>(request);

        result.Should().NotBeNull();
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(25);
        result.Search.Should().Be("bebidas");
    }
}
