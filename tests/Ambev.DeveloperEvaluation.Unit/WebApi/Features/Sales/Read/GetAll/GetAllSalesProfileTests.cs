using Ambev.DeveloperEvaluation.Application.Sales.Querys.GetAll;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common.Response;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetAll;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Read.GetAll;

public class GetAllSalesProfileTests
{
    private readonly IMapper _mapper;

    public GetAllSalesProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetAllSalesProfile>();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void GetAllSalesProfile_ConfigurationIsValid()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetAllSalesProfile>();
        });

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_Map_GetAllSalesPaginationRequest_To_GetAllSalesQuery()
    {
        // Arrange
        var request = new GetAllSalesPaginationRequest
        {
            Page = 2,
            PageSize = 15
        };

        // Act
        var query = _mapper.Map<GetAllSalesQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.Page.Should().Be(request.Page);
        query.PageSize.Should().Be(request.PageSize);
    }
}
