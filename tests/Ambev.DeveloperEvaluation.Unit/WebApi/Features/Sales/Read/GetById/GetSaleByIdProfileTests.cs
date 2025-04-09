using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.Querys.GetById;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common.Response;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetById;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Read.GetById;

public class GetSaleByIdProfileTests
{
    private readonly IMapper _mapper;

    public GetSaleByIdProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetSaleByIdProfile>();
            cfg.CreateMap<SaleItem, SaleItemResponse>();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void GetSaleByIdProfile_ConfigurationIsValid()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GetSaleByIdProfile>();
        });

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_Map_GetSaleByIdRequest_To_GetSaleByIdQuery()
    {
        // Arrange
        var request = new GetSaleByIdRequest(Guid.NewGuid());

        // Act
        var query = _mapper.Map<GetSaleByIdQuery>(request);

        // Assert
        query.Should().NotBeNull();
        query.Id.Should().Be(request.Id);
    }
}
