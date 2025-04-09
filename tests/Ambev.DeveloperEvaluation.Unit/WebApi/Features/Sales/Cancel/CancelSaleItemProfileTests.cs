using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.SaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.SaleItem;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Cancel;

public class CancelSaleItemProfileTests
{
    private readonly IMapper _mapper;

    public CancelSaleItemProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CancelSaleItemProfile>();
        });

        _mapper = config.CreateMapper();
    }

    [Fact]
    public void CancelSaleItemProfile_ConfigurationIsValid()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CancelSaleItemProfile>();
        });

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_Map_CancelSaleItemsRequest_To_CancelSaleItemsCommand_Correctly()
    {
        var saleId = Guid.NewGuid();
        var itemIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var request = new CancelSaleItemsRequest(saleId, itemIds);

        var command = _mapper.Map<CancelSaleItemsCommand>(request);

        command.Should().NotBeNull();
        command.SaleId.Should().Be(saleId);
        command.ItemIds.Should().BeEquivalentTo(itemIds);
    }
}
