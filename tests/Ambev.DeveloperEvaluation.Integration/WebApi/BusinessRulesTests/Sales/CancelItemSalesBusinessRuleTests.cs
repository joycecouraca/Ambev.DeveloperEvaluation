using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Integration.Fixture;
using Ambev.DeveloperEvaluation.Integration.Utils;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.WebApi.BusinessRulesTests.Sales;

public class CancelItemSalesBusinessRuleTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly DefaultContext _context;
    private readonly IJwtTokenGenerator _jwt;

    public CancelItemSalesBusinessRuleTests(CustomWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        _jwt = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();


        var token = _jwt.GenerateToken(_context.Users.First());
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CancelSaleItems_ShouldReturnBadRequest_WhenItemDoesNotBelongToSale()
    {
        // Arrange
        var customer = _context.Users.First();
        var product = _context.Products.First();

        var sale = Sale.Create(customer, customer, DateTime.UtcNow, "Branch Test");
        var unrelatedItem = new SaleItem(product, 10); // item NÃO adicionado à venda
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var payload = JsonSerializer.Serialize(new[] { unrelatedItem.Id });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/sales/{sale.Id}/items/cancel", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationErrorsAsync(response);
        details.Should().Contain(e => e.ToLower().Contains("no valid items found"));
    }

    [Fact]
    public async Task CancelSaleItems_ShouldReturnOk_WhenItemsAreValid()
    {
        // Arrange
        var customer = _context.Users.First();
        var product = _context.Products.First();

        var item = new SaleItem(product, 1);
        var sale = Sale.Create(customer, customer, DateTime.UtcNow, "Branch Test");
        sale.AddItems([item]);

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var payload = JsonSerializer.Serialize(new[] { item.Id });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/sales/{sale.Id}/items/cancel", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        _context.ChangeTracker.Clear(); // limpa o cache de tracking
        var updatedItem = _context.SaleItems.Find(item.Id);
        updatedItem!.Status.Should().Be(SaleItemStatus.Cancelled);
    }

    [Fact]
    public async Task CancelSaleItems_ShouldReturnBadRequest_WhenItemIsAlreadyCancelled()
    {
        // Arrange
        var customer = _context.Users.First();
        var product = _context.Products.First();

        var item = new SaleItem(product, 1);
        item.Cancel(customer); // já cancelado
        var sale = Sale.Create(customer, customer, DateTime.UtcNow, "Branch Test");
        sale.AddItems([item]);

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();
        
        var payload = JsonSerializer.Serialize(new[] { item.Id });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/sales/{sale.Id}/items/cancel", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationErrorsAsync(response);
        details.Should().Contain(e => e.ToLower().Contains("already cancelled"));
    }

    [Fact]
    public async Task CancelSaleItems_ShouldReturnNotFound_WhenSaleDoesNotExist()
    {
        // Arrange       
        var payload = JsonSerializer.Serialize(new[] { Guid.NewGuid() });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var nonExistentSaleId = Guid.NewGuid();

        // Act
        var response = await _client.PatchAsync($"/api/sales/{nonExistentSaleId}/items/cancel", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationErrorsAsync(response);
        details.Should().Contain(e => e.ToLower().Contains("sale not found."));
    }
}
