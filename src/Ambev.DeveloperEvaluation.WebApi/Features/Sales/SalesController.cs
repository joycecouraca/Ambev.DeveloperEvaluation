using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Products.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Create;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Create;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SalesController : BaseController
{
    private readonly IMapper _mapper;
    private readonly IRequestDispatcher _dispatcher;

    public SalesController(IMapper mapper, IRequestDispatcher dispatcher)
    {
        _mapper = mapper;
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Creates a new sale
    ///// </summary>
    //[HttpPost]
    //[ProducesResponseType(typeof(ApiResponseWithData<CreateProductsResponse>), StatusCodes.Status201Created)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> CreateProduct([FromBody] CreateSalesRequest request, CancellationToken cancellationToken)
    //{
    //    //return await SendValidated<CreateSalesRequest, CreateProductsCommand, Result<CreateProductDto>>(_dispatcher, request,
    //    //    result => Created(string.Empty, new ApiResponseWithData<CreateProductsResponse>
    //    //    {
    //    //        Success = true,
    //    //        Message = "Product created successfully",
    //    //        Data = _mapper.Map<CreateProductsResponse>(result.Value)
    //    //    }), cancellationToken);
    //}
}