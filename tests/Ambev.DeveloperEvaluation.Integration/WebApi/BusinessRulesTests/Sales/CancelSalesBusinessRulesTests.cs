using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Integration.Fixture;
using Ambev.DeveloperEvaluation.Integration.Utils;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.WebApi.BusinessRulesTests.Sales;

public class CancelSalesBusinessRulesTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly DefaultContext _context;
    private readonly IJwtTokenGenerator _jwt;

    public CancelSalesBusinessRulesTests(CustomWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        _jwt = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();


        var token = _jwt.GenerateToken(_context.Users.First());
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CancelSale_ShouldReturnBadRequest_WhenSaleIsAlreadyCanceled()
    {
        // Arrange        
        var user = _context.Users.First();
        var product = _context.Products.First();

        var sale = Sale.Create(user, user, DateTime.UtcNow, "Test Branch");
        var item = new SaleItem(product, 1);
        sale.AddItems([item]);
        sale.Cancel(user);

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.PatchAsync($"/api/sales/{sale.Id}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationErrorsAsync(response);
        details.Should().Contain(e => e.ToLower().Contains("cancelled"));
    }

    [Fact]
    public async Task CancelSale_ShouldReturnNotFound_WhenSaleDoesNotExist()
    {
        // Act
        var response = await _client.PatchAsync($"/api/sales/{Guid.NewGuid()}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var details = await ValidationHelper.GetValidationErrorsAsync(response);
        details.Should().Contain(e => e.ToLower().Contains("not found"));
    }

    [Fact]
    public async Task CancelSale_ShouldReturnOk_WhenSaleIsSuccessfullyCanceled()
    {
        // Arrange       
        var customer = _context.Users.First();
        var product = _context.Products.First();

        var sale = Sale.Create(customer, customer, DateTime.UtcNow, "Main Branch");
        var item = new SaleItem(product, 5);
        sale.AddItems([item]);

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        // apenas pra checar se a venda foi salva
        var exists = await _context.Sales.AnyAsync(s => s.Id == sale.Id);
        exists.Should().BeTrue();

        // Act
        var response = await _client.PatchAsync($"/api/sales/{sale.Id}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        _context.ChangeTracker.Clear(); // limpa o cache de tracking
        var updatedSale = await _context.Sales.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sale.Id);
        updatedSale.Should().NotBeNull(); // pra garantir antes de acessar
        updatedSale!.CanBeCancelled.Should().BeTrue();
    }
}
