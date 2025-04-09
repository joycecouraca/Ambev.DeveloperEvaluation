using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Integration.Fixture;
using Ambev.DeveloperEvaluation.Integration.Utils;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.WebApi.BusinessRulesTests.Sales;

public class GetSalesBusinessRuleTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly DefaultContext _context;
    private readonly IJwtTokenGenerator _jwt;

    public GetSalesBusinessRuleTests(CustomWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        _jwt = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();


        var token = _jwt.GenerateToken(_context.Users.First());
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GetSales_ShouldReturnSalesWithPagination()
    {
        // Arrange
        var customer = _context.Users.First();
        var product = _context.Products.Where(c=> c.Name == "Camisa Polo").First();

        // cria vendas
        for (int i = 0; i < 5; i++)
        {
            var sale = Sale.Create(customer, customer, DateTime.UtcNow, $"Branch {i}");
            sale.AddItems([new SaleItem(product, 1)]);
            _context.Sales.Add(sale);
        }

        await _context.SaveChangesAsync();

        var query = "?Page=1&PageSize=3";

        // Act
        var response = await _client.GetAsync($"/api/sales{query}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(content);

        var data = document.RootElement.GetProperty("items");
        data.GetArrayLength().Should().BeLessOrEqualTo(3);
    }

    [Fact]
    public async Task GetSales_ShouldReturnEmpty_WhenNoResultsMatchSearch()
    {
        var query = "?Page=1&PageSize=10&Search=naoencontra";
        var response = await _client.GetAsync($"/api/sales{query}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        var items = root.GetProperty("items");
        items.GetArrayLength().Should().Be(0);
    }

    [Fact]
    public async Task GetSales_ShouldFilterByUsernameOrStatus()
    {
        var knownUser = _context.Users.First(); // Use esse helper
        var username = knownUser.Username;

        var query = $"?Page=1&PageSize=10&Search={username.Substring(0, 3)}"; // busca parcial
        var response = await _client.GetAsync($"/api/sales{query}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        var items = root.GetProperty("items");
        items.GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetSaleById_ShouldReturnSale_WhenIdIsValid()
    {
        var sale = _context.Sales.First(); // usa helper
        var response = await _client.GetAsync($"/api/sales/{sale.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        var data = root.GetProperty("data");
        data.GetProperty("id").GetGuid().Should().Be(sale.Id);
    }

    [Fact]
    public async Task GetSaleById_ShouldReturnBadRequest_WhenSaleDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/sales/{nonExistentId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationErrorsAsync(response);
        details.Should().Contain(e => e.ToLower().Contains("sale"));
    }
}
