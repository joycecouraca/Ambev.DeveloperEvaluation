using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetAll;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products.Read.GetAll;

public class GetAllProductsRequestValidatorTests
{
    private readonly GetAllProductsRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Request_Is_Valid()
    {
        var request = new GetAllProductsPaginationRequest
        {
            Page = 1,
            PageSize = 50,
            Search = "bebidas"
        };

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, "Page number must be greater than 0.")]
    [InlineData(-1, "Page number must be greater than 0.")]
    public void Should_Fail_When_Page_Is_Invalid(int page, string errorMessage)
    {
        var request = new GetAllProductsPaginationRequest
        {
            Page = page,
            PageSize = 10,
            Search = null
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Page)
              .WithErrorMessage(errorMessage);
    }

    [Theory]
    [InlineData(0, "Page size must be greater than 0.")]
    [InlineData(101, "Page size must be 100 or less.")]
    public void Should_Fail_When_PageSize_Is_Invalid(int pageSize, string errorMessage)
    {
        var request = new GetAllProductsPaginationRequest
        {
            Page = 1,
            PageSize = pageSize,
            Search = null
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.PageSize)
              .WithErrorMessage(errorMessage);
    }

    [Fact]
    public void Should_Fail_When_Search_Is_Too_Long()
    {
        var request = new GetAllProductsPaginationRequest
        {
            Page = 1,
            PageSize = 10,
            Search = new string('a', 51)
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(r => r.Search)
              .WithErrorMessage("Search category must be 50 characters or less.");
    }
}
