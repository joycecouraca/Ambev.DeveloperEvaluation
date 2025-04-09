using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Sales.Common.Dtos;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Integration.Fixture;
using Ambev.DeveloperEvaluation.Integration.Utils;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.WebApi.Controllers;

public class SalesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly DefaultContext _context;


    public SalesControllerTests(CustomWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();

        _context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var jwtGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

        var user = _context.Users.First();

        var token = jwtGenerator.GenerateToken(user);

        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    #region Success
    [Fact]
    public async Task CreateSale_ShouldReturnCreated_WhenRequestIsValid()
    {
        var customerId = _context.Users.First().Id;
        var productId = _context.Products.First().Id;

        var request = new
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "Unidade Teste",
            CustomerId = customerId,
            Items = new[]
            {
                new { ProductId = productId, Quantity = 2 }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/sales", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Sale created successfully");
    }

    [Fact]
    public async Task CancelSale_ShouldReturnOk_WhenSeededSaleExists()
    {
        var sale = _context.Sales.LastOrDefault();
        sale.Should().NotBeNull("O seed deve ter criado ao menos uma venda para esse teste");

        var saleId = sale!.Id;

        var response = await _client.PatchAsync($"/api/sales/{saleId}/cancel", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Sale cancelled successfully");
    }

    [Fact]
    public async Task CancelSaleItems_ShouldReturnOk_WhenItemsAreCancelledSuccessfully()
    {
        // Arrange        
        var sale = _context.Sales
            .Include(s => s.Items)
            .FirstOrDefault(s => s.Items.Any());

        sale.Should().NotBeNull("precisamos de uma venda com itens para testar o cancelamento");

        var saleId = sale!.Id;
        var itemIds = sale.Items.Select(i => i.Id).ToList();
        itemIds.Should().NotBeEmpty("esperamos que a venda tenha itens com IDs válidos");

        var request = JsonSerializer.Serialize(itemIds);
        var content = new StringContent(request, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/sales/{saleId}/items/cancel", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("Item(s) cancelled successfully");
    }

    [Fact]
    public async Task GetAllSales_ShouldReturnPaginatedSales_WhenRequestIsValid()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var url = $"/api/sales?page={page}&pageSize={pageSize}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("items", "esperamos que a resposta contenha a lista de vendas");
        body.Should().Contain("page");
        body.Should().Contain("pageSize");
        body.Should().Contain("totalCount");
    }

    [Fact]
    public async Task GetSaleById_ShouldReturnOk_WhenSaleExists()
    {
        // Arrange       
        var sale = _context.Sales
            .Include(s => s.Items)
            .FirstOrDefault(s => s.Items.Any());

        sale.Should().NotBeNull("precisamos de uma venda com itens para buscar por ID");

        var saleId = sale!.Id;

        // Act
        var response = await _client.GetAsync($"/api/sales/{saleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().Contain("Sale retrieved successfully");
        responseBody.Should().Contain(sale.BranchName);
    }
    #endregion

    #region ValidationInvalid StatusCode 400
    [Fact]
    public async Task CreateSale_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "",
            CustomerId = Guid.Empty,
            Items = new object[] { }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/sales", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationDetailsAsync(response);

        details.Should().Contain(e => e.ToLower().Contains("branch name"));
        details.Should().Contain(e => e.ToLower().Contains("customerid"));
        details.Should().Contain(e => e.ToLower().Contains("item"));
    }

    [Fact]
    public async Task CancelSaleItems_ShouldReturnBadRequest_WhenItemIdsAreEmpty()
    {
        // Arrange        
        var sale = _context.Sales.FirstOrDefault();
        sale.Should().NotBeNull();

        var saleId = sale!.Id;

        var itemIds = Array.Empty<Guid>();
        var content = new StringContent(JsonSerializer.Serialize(itemIds), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PatchAsync($"/api/sales/{saleId}/items/cancel", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationDetailsAsync(response);

        details.Should().Contain(e => e.ToLower().Contains("item"));
    }

    [Fact]
    public async Task GetAllSales_ShouldReturnBadRequest_WhenPageIsInvalid()
    {
        // Arrange: Página 0 e tamanho negativo são inválidos
        var query = "?Page=0&PageSize=-5";
        var response = await _client.GetAsync($"/api/sales{query}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationDetailsAsync(response);

        details.Should().Contain(e => e.ToLower().Contains("page"));
        details.Should().Contain(e => e.ToLower().Contains("pagesize"));
    }

    [Fact]
    public async Task GetSaleById_ShouldReturnBadRequest_WhenIdIsEmpty()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act
        var response = await _client.GetAsync($"/api/sales/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationDetailsAsync(response);

        details.Should().Contain(e => e.ToLower().Contains("id"));
    }

    [Fact]
    public async Task CancelSale_ShouldReturnBadRequest_WhenIdIsEmpty()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act
        var response = await _client.PatchAsync($"/api/sales/{invalidId}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var details = await ValidationHelper.GetValidationDetailsAsync(response);

        details.Should().Contain(e => e.ToLower().Contains("id"));
    }
    #endregion

    #region Exception StatusCode 500
    [Fact]
    public async Task CreateSale_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        // Arrange: cria factory com mock do dispatcher
        var factory = new MockedWebApplicationFactory(overrideServices =>
        {
            var mockDispatcher = new Mock<IRequestDispatcher>();
            mockDispatcher
                .Setup(d => d.SendValidatedAsync<CreateSalesRequest, CreateSalesCommand, Result<SaleDto>>(
                    It.IsAny<CreateSalesRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Simulated exception for test"));

            overrideServices.AddScoped(_ => mockDispatcher.Object);
        });

        var client = TestHelper.CreateAuthenticatedClient(factory, out var context);
        var product = context.Products.First();
        var customerId = context.Users.First().Id;

        var request = new
        {
            SoldAt = DateTime.UtcNow,
            BranchName = "Branch X",
            CustomerId = customerId,
            Items = new[] { new { ProductId = product.Id, Quantity = 1 } }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/sales", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var doc = await TestHelper.ParseJsonAsync(response);

        doc.RootElement.GetProperty("success").GetBoolean().Should().BeFalse();
        doc.RootElement.GetProperty("message").GetString().Should().Be("An unexpected error occurred.");
        doc.RootElement.GetProperty("errors")[0].GetProperty("error").GetString().Should().Be("UnhandledException");
        doc.RootElement.GetProperty("errors")[0].GetProperty("detail").GetString().Should().Contain("Simulated exception for test");
    }
    #endregion

}