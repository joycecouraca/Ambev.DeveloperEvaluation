using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Products.Query.GetAll;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Querys.GetAll;

public class GetAllProductsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<CreateProductsCommandHandler>> _loggerMock = new();

    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {
        _handler = new GetAllProductsQueryHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedProducts_WhenProductsExist()
    {
        const int page = 1;
        const int pageSize = 10;

        var products = new List<Product>
        {
            new("Product 1", 10.0m, "Description", 1, ProductCategory.Others),
            new("Product 2", 20.0m, "Description", 1, ProductCategory.Others)
        };

        var paginatedList = new PaginatedList<Product>
        {
            Items = products,
            Page = page,
            PageSize = pageSize,
            TotalCount = products.Count
        };

        var query = new GetAllProductsQuery { Page = page, PageSize = pageSize };

        _unitOfWorkMock
            .Setup(repo => repo.Products.GetPaginatedAsync(
                page,
                pageSize,
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(paginatedList);

        _mapperMock
            .Setup(m => m.Map<PaginatedList<ProductDto>>(It.IsAny<PaginatedList<Product>>()))
            .Returns(new PaginatedList<ProductDto>
            {
                Items = new List<ProductDto>(),
                Page = page,
                PageSize = pageSize,
                TotalCount = products.Count
            });

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(0); 
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown()
    {
        const int page = 1;
        const int pageSize = 10;

        var query = new GetAllProductsQuery { Page = page, PageSize = pageSize };

        _unitOfWorkMock
            .Setup(repo => repo.Products.GetPaginatedAsync(
                page,
                pageSize,
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                It.IsAny<CancellationToken>()
            ))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Failed to load products");
    }
}
