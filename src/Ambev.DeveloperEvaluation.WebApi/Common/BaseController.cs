using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using Ambev.DeveloperEvaluation.Common.Validation;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using IResult = Ambev.DeveloperEvaluation.Application.Common.Abstractions.IResult;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public abstract class BaseController : ControllerBase
{
    protected async Task<IActionResult> SendValidated<TRequest, TCommand, TResponse>(IRequestDispatcher dispatcher, TRequest request, Func<TResponse, IActionResult> onSuccess,
        CancellationToken cancellationToken)
        where TCommand : class, IRequest<TResponse>
        where TResponse : IResult
    {
        try
        {
            var result = await dispatcher.SendValidatedAsync<TRequest, TCommand, TResponse>(request, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Errors =
                    [
                        new ValidationErrorDetail
                        {
                            Error = result.Error ?? "ExceptionError",
                            Detail = result.Message ?? "Error processing the request"
                        }
                    ]
                });
            }

            return onSuccess(result);
        }
        catch (ValidationException ex)
        {
            var errorResponse = new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = ex.Errors.Select(e => (ValidationErrorDetail)e)
            };

            return BadRequest(errorResponse);
        }
        catch (Exception ex)
        {
            var errorResponse = new ApiResponse
            {
                Success = false,
                Message = "An unexpected error occurred.",
                Errors =
                [
                    new ValidationErrorDetail
                    {
                        Error = "UnhandledException",
                        Detail = ex.Message
                    }
                ]
            };

            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }
}
