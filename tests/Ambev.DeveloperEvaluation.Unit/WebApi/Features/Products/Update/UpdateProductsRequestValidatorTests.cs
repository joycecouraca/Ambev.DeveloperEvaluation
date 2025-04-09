using Ambev.DeveloperEvaluation.WebApi.Features.Products.Update;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Update;

public class UpdateProductsRequestValidatorTests
{
    private readonly UpdateProductsRequestValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        var model = new UpdateProductsRequest
        {
            Id = Guid.Empty,
            Name = "Test Product",
            Description = "Sample Description",
            Price = 10.0m,
            Quantity = 5,
            Category = "Others" 
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Id is required.");
    }

    [Fact]
    public void Should_Have_Errors_When_Fields_Are_Invalid()
    {
        var model = new UpdateProductsRequest
        {
            Id = Guid.NewGuid(),
            Name = "", 
            Description = "", 
            Price = 0, 
            Quantity = 0, 
            Category = "InvalidCategory"
        };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);
        result.ShouldHaveValidationErrorFor(x => x.Price);
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Model_Is_Valid()
    {
        var model = new UpdateProductsRequest
        {
            Id = Guid.NewGuid(),
            Name = "Valid Product",
            Description = "A valid product description.",
            Price = 29.99m,
            Quantity = 5,
            Category = "Others"
        };

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
