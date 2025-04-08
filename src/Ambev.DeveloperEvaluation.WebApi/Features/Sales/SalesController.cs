using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.Sale;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Cancel.SaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Create.Dtos;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.Sale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Cancel.SaleItem;
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

    [HttpPatch("{saleId:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale(Guid saleId, CancellationToken cancellationToken)
    {
        var command = new CancelSaleRequest(saleId);

        return await SendValidated<CancelSaleRequest, CancelSaleCommand, Result<Guid>>(
            _dispatcher,
            command,
            result => Ok(new ApiResponse
            {
                Success = true,
                Message = "Sale cancelled successfully"
            }),
            cancellationToken);
    }

    [HttpPatch("{saleId}/items/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelItem(Guid saleId, [FromBody] List<Guid> itemId, CancellationToken cancellationToken)
    {
        var request = new CancelSaleItemsRequest(saleId, itemId);

        return await SendValidated<CancelSaleItemsRequest, CancelSaleItemsCommand, Result<CancelSaleItemResponse>>(
                _dispatcher,
                request,
                result => Ok(new ApiResponseWithData<CancelSaleItemResponse>
                {
                    Success = true,
                    Message = "Item(s) cancelled successfully",
                    Data = result.Value
                }),
                cancellationToken
        );
    }
}