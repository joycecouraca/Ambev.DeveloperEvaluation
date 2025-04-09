using Ambev.DeveloperEvaluation.Application.Products.Query.GetById;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetById;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Read.GetById;

public class GetProductByIdProfileTests
{
    private readonly IMapper _mapper;

    public GetProductByIdProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<GetProductByIdProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_GetProductByIdRequest_To_GetProductByIdQuery()
    {
        var request = new GetProductByIdRequest
        {
            Id = Guid.NewGuid()
        };

        var result = _mapper.Map<GetProductByIdQuery>(request);

        result.Should().NotBeNull();
        result.Id.Should().Be(request.Id);
    }
}
