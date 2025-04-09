using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Delete;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Products.Query.GetAll;
using Ambev.DeveloperEvaluation.Application.Products.Query.GetById;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.WebApi.Features.Products;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Common.Response;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Delete;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetAll;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetById;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Update;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Products;

public class ProductsControllerTests
{
    private readonly Mock<IRequestDispatcher> _dispatcherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _dispatcherMock = new Mock<IRequestDispatcher>();
        _mapperMock = new Mock<IMapper>();
        _controller = new ProductsController(_mapperMock.Object, _dispatcherMock.Object);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated_WhenValid()
    {
        // Arrange
        var request = new CreateProductsRequest { Name = "Test", Description = "Desc" };
        var resultDto = new ProductDto { Id = Guid.NewGuid(), Name = "Test", Description = "Desc" };
        var result = Result<ProductDto>.Success(resultDto);

        _dispatcherMock
            .Setup(x => x.SendValidatedAsync<CreateProductsRequest, CreateProductsCommand, Result<ProductDto>>(
                It.IsAny<CreateProductsRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProductDto>.Success(new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Product"
                // Adicione os campos necessários aqui
            }));

        _mapperMock
            .Setup(x => x.Map<ProductResponse>(It.IsAny<ProductDto>()))
            .Returns(new ProductResponse { Name = "Test", Description = "Desc" });

        // Act
        var response = await _controller.CreateProduct(request, CancellationToken.None);

        // Assert
        var created = response.Should().BeOfType<CreatedResult>().Subject;
        created.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenValid()
    {
        // Arrange
        var request = new UpdateProductsRequest { Id = Guid.NewGuid(), Name = "Updated", Description = "Updated Desc" };
        var resultDto = new ProductDto { Id = request.Id, Name = request.Name, Description = request.Description };
        var result = Result<ProductDto>.Success(resultDto);

        _dispatcherMock
            .Setup(x => x.SendValidatedAsync<UpdateProductsRequest, UpdateProductsCommand, Result<ProductDto>>(It.IsAny<UpdateProductsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        _mapperMock
            .Setup(x => x.Map<ProductResponse>(It.IsAny<ProductDto>()))
            .Returns(new ProductResponse { Id = request.Id, Name = "Updated", Description = "Updated Desc" });

        // Act
        var response = await _controller.UpdateProduct(request, CancellationToken.None);

        // Assert
        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnOk_WhenValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var result = Result<Guid>.Success(id);

        _dispatcherMock
            .Setup(x => x.SendValidatedAsync<DeleteProductRequest, DeleteProductsCommand, Result<Guid>>(It.IsAny<DeleteProductRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.DeleteProduct(id, CancellationToken.None);

        // Assert
        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetAllProduct_ShouldReturnPaginatedList()
    {
        // Arrange
        var request = new GetAllProductsPaginationRequest { Page = 1, PageSize = 10 };
        var resultDto = new PaginatedList<ProductDto>
        {
            Page = 1,
            PageSize = 10,
            TotalCount = 1,
            Items = [new() { Id = Guid.NewGuid(), Name = "Prod", Description = "Desc" }]
        };

        _dispatcherMock
            .Setup(x => x.SendValidatedAsync<GetAllProductsPaginationRequest, GetAllProductsQuery, Result<PaginatedList<ProductDto>>>(It.IsAny<GetAllProductsPaginationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PaginatedList<ProductDto>>.Success(resultDto));

        _mapperMock
            .Setup(x => x.Map<List<ProductResponse>>(It.IsAny<List<ProductDto>>()))
            .Returns([new() { Name = "Prod", Description = "Desc" }]);

        // Act
        var response = await _controller.GetAllProduct(request, CancellationToken.None);

        // Assert
        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new GetProductByIdRequest { Id = id };
        var dto = new ProductDto { Id = id, Name = "Produto", Description = "Descrição" };

        _dispatcherMock
            .Setup(x => x.SendValidatedAsync<GetProductByIdRequest, GetProductByIdQuery, Result<ProductDto>>(It.IsAny<GetProductByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProductDto>.Success(dto));

        _mapperMock
            .Setup(x => x.Map<ProductResponse>(dto))
            .Returns(new ProductResponse { Name = dto.Name, Description = dto.Description });

        // Act
        var result = await _controller.GetProductById(id, CancellationToken.None);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
    }
}