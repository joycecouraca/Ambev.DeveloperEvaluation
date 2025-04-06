using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetById;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Read.GetById;

public class GetProductByIdValidatorTests
{
    private readonly GetProductByIdValidator _validator;

    public GetProductByIdValidatorTests()
    {
        _validator = new GetProductByIdValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        var request = new GetProductByIdRequest { Id = Guid.Empty };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Product ID must not be empty.");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Valid()
    {
        var request = new GetProductByIdRequest { Id = Guid.NewGuid() };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
