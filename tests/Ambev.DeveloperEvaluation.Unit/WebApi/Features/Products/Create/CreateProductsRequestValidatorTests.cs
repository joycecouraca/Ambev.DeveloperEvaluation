using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Create;

public class CreateProductsRequestValidatorTests
{
    private readonly CreateProductsRequestValidator _validator;

    public CreateProductsRequestValidatorTests()
    {
        _validator = new CreateProductsRequestValidator();
    }

    [Fact]
    public void Validator_Should_Pass_For_Valid_Request()
    {
        var request = new CreateProductsRequest
        {
            Name = "Valid Product",
            Description = "A valid product description.",
            Price = 10.99m,
            Quantity = 5,
            Category = nameof(ProductCategory.Electronics)
        };

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Description", 10.99, 5, "Electronics")]
    [InlineData("Pr", "Description", 10.99, 5, "Electronics")]
    [InlineData("Product", "", 10.99, 5, "Electronics")]
    [InlineData("Product", "De", 10.99, 5, "Electronics")]
    [InlineData("Product", "Valid Description", 0, 5, "Electronics")]
    [InlineData("Product", "Valid Description", 10.99, 0, "Electronics")]
    [InlineData("Product", "Valid Description", 10.99, 5, "InvalidCategory")]
    public void Validator_Should_Fail_For_Invalid_Inputs(string name, string description, decimal price, int quantity, string category)
    {
        var request = new CreateProductsRequest
        {
            Name = name,
            Description = description,
            Price = price,
            Quantity = quantity,
            Category = category
        };

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeFalse();
    }
}