using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Read.GetById;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Sales.Read.GetById;

public class GetSaleByIdRequestTests
{
    [Fact]
    public void Constructor_Should_Set_Id_Correctly()
    {
        var guid = Guid.NewGuid();

        var request = new GetSaleByIdRequest(guid);

        request.Id.Should().Be(guid);
    }
}
