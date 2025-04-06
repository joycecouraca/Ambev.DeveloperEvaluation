using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Delete;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Update.Dtos;
using Ambev.DeveloperEvaluation.Application.Products.Common;
using Ambev.DeveloperEvaluation.Application.Products.Query.GetAll;
using Ambev.DeveloperEvaluation.Application.Products.Query.GetById;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Delete;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetAll;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Read.GetById;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Update;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRequestDispatcher _dispatcher;

        public ProductsController(IMapper mapper, IRequestDispatcher dispatcher)
        {
            _mapper = mapper;
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="request">The product creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created product details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<CreateProductsResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductsRequest request, CancellationToken cancellationToken)
        {
            return await SendValidated<CreateProductsRequest, CreateProductsCommand, Result<CreateProductDto>>(_dispatcher, request,
                result => Created(string.Empty, new ApiResponseWithData<CreateProductsResponse>
                {
                    Success = true,
                    Message = "Product created successfully",
                    Data = _mapper.Map<CreateProductsResponse>(result.Value)
                }), cancellationToken);
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="request">The product update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated product details</returns>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponseWithData<UpdateProductsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductsRequest request, CancellationToken cancellationToken)
        {
            return await SendValidated<UpdateProductsRequest, UpdateProductsCommand, Result<UpdateProductDto>>(_dispatcher, request,
                result => Ok(new ApiResponseWithData<UpdateProductsResponse>
                {
                    Success = true,
                    Message = "Product updated successfully",
                    Data = _mapper.Map<UpdateProductsResponse>(result.Value)
                }), cancellationToken);
        }

        /// <summary>
        /// Deletes a product by their ID
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success response if the product was deleted</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new DeleteProductRequest { Id = id };
            return await SendValidated<DeleteProductRequest, DeleteProductsCommand, Result<Guid>>(_dispatcher, request,
                result => Ok(new ApiResponseWithData<Guid>
                {
                    Success = true,
                    Message = "Product delete successfully",
                    Data = result.Value
                }), cancellationToken);
        }

        /// <summary>
        /// Retrieves all products with pagination support.
        /// </summary>
        /// <param name="request">The pagination request parameters.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A paginated list of products.</returns>
        [HttpGet(Name = nameof(GetAllProduct))]
        [ProducesResponseType(typeof(ApiResponseWithData<PaginatedList<ProductResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetAllProduct([FromQuery] GetAllProductsPaginationRequest request, CancellationToken cancellationToken)
        {
            return await SendValidated<GetAllProductsPaginationRequest, GetAllProductsQuery, Result<PaginatedList<GetProductDto>>>(
                _dispatcher,
                request,
                result => Ok(new PaginatedList<ProductResponse>
                {
                    Page = result.Value!.Page,
                    PageSize = result.Value.PageSize,
                    TotalCount = result.Value.TotalCount,
                    Items = _mapper.Map<List<ProductResponse>>(result.Value.Items)
                }),
                cancellationToken);
        }

        /// <summary>
        /// Retrieves all products with pagination support.
        /// </summary>
        /// <param name="request">The pagination request parameters.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A paginated list of products.</returns>        
        [HttpGet("{id}", Name = nameof(GetProductById))]
        [ProducesResponseType(typeof(ApiResponseWithData<PaginatedList<ProductResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProductById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new GetProductByIdRequest { Id = id };
            return await SendValidated<GetProductByIdRequest, GetProductByIdQuery, Result<GetProductDto>>(
                _dispatcher,
                request,
                result => Ok(new ApiResponseWithData<ProductResponse>
                {
                    Success = true,
                    Message = "Product recovered successfully",
                    Data = _mapper.Map<ProductResponse>(result.Value)
                }), cancellationToken);
        }
    }
}
