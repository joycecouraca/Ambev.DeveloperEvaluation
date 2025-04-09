using Ambev.DeveloperEvaluation.Application.Products.Commands.Delete;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Commands.Delete;

public class DeleteProductsCommandTests
{
    [Fact]
    public void Constructor_ShouldSetId()
    {
        var expectedId = Guid.NewGuid();

        var command = new DeleteProductsCommand(expectedId);

        command.Id.Should().Be(expectedId);
    }
}
