using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Integration.Fixture;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.WebApi.BusinessRulesTests.Products;

public class CreateProductsBusinessRulesTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly DefaultContext _context;
    private readonly IJwtTokenGenerator _jwt;

    public CreateProductsBusinessRulesTests(CustomWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        _jwt = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

        var token = _jwt.GenerateToken(_context.Users.First());
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenProductWithSameNameExists()
    {
        // Arrange - cria um produto no banco com nome fixo
        var existingProduct = new Product(
            name: "Produto Único",
            price: 99.90m,
            description: "Produto de teste",
            quantity: 10,
            category: ProductCategory.Electronics
        );

        _context.Products.Add(existingProduct);
        await _context.SaveChangesAsync();

        // Act - tenta criar novo produto com o mesmo nome
        var request = new
        {
            Name = "Produto Único",
            Price = 120.00m,
            Description = "Tentativa de duplicação",
            Quantity = 5,
            Category = ProductCategory.Electronics.ToString()
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadAsStringAsync();
        body.ToLower().Should().Contain("already exists");
    }

    [Fact]
    public async Task UpdateProduct_ShouldSucceed_WhenDataIsValid()
    {
        var product = _context.Products.First();

        var request = new
        {
            Id = product.Id,
            Name = "Nome Atualizado",
            Price = 299.99m,
            Description = "Descrição nova",
            Quantity = 20,
            Category = product.Category.ToString()
        };

        var content = JsonContent.Create(request);

        var response = await _client.PutAsync("/api/products", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        _context.ChangeTracker.Clear(); // limpa o cache de tracking

        var updated = await _context.Products.FindAsync(product.Id);
        updated!.Name.Should().Be("Nome Atualizado");
    }    

    [Fact]
    public async Task DeleteProduct_ShouldSucceed_WhenProductExists()
    {
        var product = new Product("Produto Excluível", 100, "Desc", 1, ProductCategory.Others);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/api/products/{product.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        _context.ChangeTracker.Clear(); // limpa o cache de tracking

        var deleted = await _context.Products.FindAsync(product.Id);
        deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteProduct_ShouldFail_WhenProductDoesNotExist()
    {
        var nonexistentId = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/products/{nonexistentId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
