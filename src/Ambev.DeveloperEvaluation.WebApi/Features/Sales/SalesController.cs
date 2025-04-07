using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;
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
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSalesResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSalesRequest request, CancellationToken cancellationToken)
    {
        return await SendValidated<CreateSalesRequest, CreateSalesCommand, Result<CreateSaleDto>>(_dispatcher, request,
            result => Created(string.Empty, new ApiResponseWithData<CreateSalesResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = _mapper.Map<CreateSalesResponse>(result.Value)
            }), cancellationToken);
    }
}