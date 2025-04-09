using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Integration.Fixture;
using Ambev.DeveloperEvaluation.Integration.Utils;
using Ambev.DeveloperEvaluation.ORM;
using Castle.Core.Resource;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.WebApi.BusinessRulesTests.Sales;

public class CreateSalesBusinessRulesTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly DefaultContext _context;
    private readonly IJwtTokenGenerator _jwt;

    public CreateSalesBusinessRulesTests(CustomWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        _jwt = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();


        var token = _jwt.GenerateToken(_context.Users.First());
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CreateSale_ShouldReturnBadRequest_WhenProductDoesNotExist()
    {
        // Arrange
        var invalidProductId = Guid.NewGuid(); // não existe no banco
        var customer = _context.Users.First();

        var request = new
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "Filial Central",
            CustomerId = customer.Id,
            Items = new[] { new { ProductId = invalidProductId, Quantity = 1 } }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/sales", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationErrorsAsync(response);
        details.Should().Contain(e => e.ToLower().Contains("product"));
    }

    [Fact]
    public async Task CreateSale_ShouldReturnBadRequest_WhenCustomerDoesNotExist()
    {
        // Arrange
        var invalidCustomerId = Guid.NewGuid(); // não existe
        var product = _context.Products.First();

        var request = new
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "Filial Central",
            CustomerId = invalidCustomerId,
            Items = new[] { new { ProductId = product.Id, Quantity = 1 } }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/sales", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationErrorsAsync(response);
        details.Should().Contain(e => e.ToLower().Contains("customer"));
    }

    [Fact]
    public async Task CreateSale_ShouldReturnBadRequest_WhenProductIsOutOfStock()
    {
        // Arrange
        var customer = _context.Users.First();
        var product = _context.Products.First();

        _context.SaveChanges();

        var request = new
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "Filial Central",
            CustomerId = customer.Id,
            Items = new[] { new { ProductId = product.Id, Quantity = 15 } }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/sales", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationErrorsAsync(response);
        details.Should().Contain(e => e.ToLower().Contains("Insufficient stock for product") || e.ToLower().Contains("stock"));
    }

    [Fact]
    public async Task CreateSale_ShouldReturnBadRequest_WhenQuantityExceedsLimit()
    {
        // Arrange        
        var product = _context.Products.First();
        var customerId = _context.Users.First().Id;


        var request = new
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "Loja B",
            CustomerId = customerId,
            Items = new[] { new { ProductId = product.Id, Quantity = 25 } } // > 20
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/sales", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationDetailsAsync(response);
        details.Should().Contain(e => e.ToLower().Contains("more than 20"));
    }
}
