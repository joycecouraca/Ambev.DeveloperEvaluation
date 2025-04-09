using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetAll;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Read.GetAll;

public class GetAllSalesRequestValidatorTests
{
    private readonly GetAllSalesRequestValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Page_Is_Less_Than_One()
    {
        var model = new GetAllSalesPaginationRequest { Page = 0, PageSize = 10 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Page)
              .WithErrorMessage("PageNumber must be greater than 0.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Should_Have_Error_When_PageSize_Is_Out_Of_Range(int pageSize)
    {
        var model = new GetAllSalesPaginationRequest { Page = 1, PageSize = pageSize };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
              .WithErrorMessage("PageSize must be between 1 and 100.");
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Request_Is_Valid()
    {
        var model = new GetAllSalesPaginationRequest { Page = 2, PageSize = 20 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
