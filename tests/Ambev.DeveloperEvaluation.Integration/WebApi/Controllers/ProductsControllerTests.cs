using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Integration.Fixture;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.WebApi.Controllers;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly DefaultContext _context;
    private readonly Product _productCurrent;


    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        _productCurrent = _context.Products.First();
        var jwtGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

        var user = _context.Users.First();

        var token = jwtGenerator.GenerateToken(user);

        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }


    [Fact]
    public async Task CreateProduct_ShouldReturnCreated_WhenValid()
    {
        var request = new
        {
            Name = "Produto Teste",
            Description = "Descrição do Produto",
            Price = 9.99m,
            Quantity = 10,
            Category = "Others"
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/products", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Product created successfully");
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenProductIsUpdated()
    {
        var request = new
        {
            _productCurrent.Id,
            Name = "Updated Name",
            _productCurrent.Description,
            Price = _productCurrent.Price + 1,
            Quantity = 10,
            Category = "Others"
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync("/api/products", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Product updated successfully");
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnOk_WhenProductIsDeleted()
    {
        var response = await _client.DeleteAsync($"/api/products/{_productCurrent.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Product delete successfully");
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnPaginatedList()
    {
        var response = await _client.GetAsync("/api/products?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("items");
        json.Should().Contain("page");
        json.Should().Contain("totalCount");
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenExists()
    {
        var productSelect = _context.Products.LastOrDefault();

        var response = await _client.GetAsync($"/api/products/{productSelect?.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("Product recovered successfully");
        json.Should().Contain(productSelect?.Name);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnBadRequest_WhenIdIsEmpty()
    {
        var response = await _client.GetAsync("/api/products/00000000-0000-0000-0000-000000000000");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnBadRequest_WhenIdIsEmpty()
    {
        var response = await _client.DeleteAsync("/api/products/00000000-0000-0000-0000-000000000000");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        var request = new
        {
            Name = "",
            Description = "",
            Price = 0
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/products", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}