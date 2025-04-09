using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetById;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Read.GetById;

public class GetSaleByIdValidatorTests
{
    private readonly GetSaleByIdValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Id_Is_Valid()
    {
        var request = new GetSaleByIdRequest (Guid.NewGuid());

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Id_Is_Empty()
    {
        var request = new GetSaleByIdRequest(Guid.Empty);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("Sale ID must be provided.");
    }
}
