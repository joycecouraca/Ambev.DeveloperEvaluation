using Ambev.DeveloperEvaluation.Application.Products.Commands.Delete;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Delete;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Delete;

public class DeleteProductsProfileTests
{
    private readonly IMapper _mapper;

    public DeleteProductsProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<DeleteProductsProfile>());

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void DeleteProductsProfile_Should_Map_DeleteProductRequest_To_DeleteProductsCommand()
    {
        var id = Guid.NewGuid();
        var request = new DeleteProductRequest
        {
            Id = id
        };

        var result = _mapper.Map<DeleteProductsCommand>(request);

        result.Should().NotBeNull();
        result.Id.Should().Be(id);
    }
}
