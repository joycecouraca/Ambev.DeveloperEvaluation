using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Create;

public class CreateSalesRequestValidatorTests
{
    private readonly CreateSalesRequestValidator _validator;

    public CreateSalesRequestValidatorTests()
    {
        _validator = new CreateSalesRequestValidator();
    }

    [Fact]
    public void Should_Pass_Validation_When_Request_Is_Valid()
    {
        var request = new CreateSalesRequest
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "Main Branch",
            CustomerId = Guid.NewGuid(),
            Items =
            [
                new() { ProductId = Guid.NewGuid(), Quantity = 5 }
            ]
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_Validation_When_BranchName_Is_Empty()
    {
        var request = new CreateSalesRequest
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "",
            CustomerId = Guid.NewGuid(),
            Items =
            [
                new() { ProductId = Guid.NewGuid(), Quantity = 5 }
            ]
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "BranchName");
    }

    [Fact]
    public void Should_Fail_Validation_When_Items_Is_Empty()
    {
        var request = new CreateSalesRequest
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "Main Branch",
            CustomerId = Guid.NewGuid(),
            Items = []
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Items");
    }

    [Fact]
    public void Should_Fail_Validation_When_Quantity_Is_Zero()
    {
        var request = new CreateSalesRequest
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "Main Branch",
            CustomerId = Guid.NewGuid(),
            Items =
            [
                new() { ProductId = Guid.NewGuid(), Quantity = 0 }
            ]
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Items[0].Quantity");
    }

    [Fact]
    public void Should_Fail_Validation_When_Quantity_Exceeds_Max()
    {
        var request = new CreateSalesRequest
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "Main Branch",
            CustomerId = Guid.NewGuid(),
            Items =
            [
                new() { ProductId = Guid.NewGuid(), Quantity = 25 }
            ]
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Items[0].Quantity");
    }
}
