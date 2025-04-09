using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Update;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Update;

public class UpdateProductsProfileTests
{
    private readonly IMapper _mapper;

    public UpdateProductsProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UpdateProductsProfile>());

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Should_Map_UpdateProductsRequest_To_UpdateProductsCommand()
    {
        var request = new UpdateProductsRequest
        {
            Id = Guid.NewGuid(),
            Name = "Updated Product",
            Price = 49.99m,
            Description = "Updated description",
            Quantity = 5,
            Category = "Others"
        };

        var command = _mapper.Map<UpdateProductsCommand>(request);

        command.Id.Should().Be(request.Id);
        command.Name.Should().Be(request.Name);
        command.Price.Should().Be(request.Price);
        command.Description.Should().Be(request.Description);
        command.Quantity.Should().Be(request.Quantity);
        command.Category.ToString().Should().Be(request.Category);
    }
}
