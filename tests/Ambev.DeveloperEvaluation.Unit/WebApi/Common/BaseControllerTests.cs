using Ambev.DeveloperEvaluation.Application.Common.Abstractions;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Common;

public class BaseControllerTests
{
    private readonly Mock<IRequestDispatcher> _dispatcherMock;
    private readonly FakeController _fakeController;
    private readonly FakeRequest _fakeRequest;

    public BaseControllerTests()
    {
        _dispatcherMock = new Mock<IRequestDispatcher>();
        _fakeController = new FakeController();
        _fakeRequest = new FakeRequest();
    }

    private class FakeRequest : IRequest<FakeResponse> { }
    private class FakeCommand : IRequest<FakeResponse> { }

    private class FakeResponse : IResult
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; }
    }

    private class FakeController : BaseController
    {
        public Task<IActionResult> CallSendValidated(IRequestDispatcher dispatcher, FakeRequest request, CancellationToken cancellationToken)
        {
            return SendValidated<FakeRequest, FakeCommand, FakeResponse>(
                dispatcher,
                request,
                res => new OkObjectResult(new { res.IsSuccess }),
                cancellationToken
            );
        }
    }

    [Fact]
    public async Task SendValidated_Should_Return_Ok_When_Success()
    {
        
        var response = new FakeResponse { IsSuccess = true };

        _dispatcherMock.Setup(d => d.SendValidatedAsync<FakeRequest, FakeCommand, FakeResponse>(_fakeRequest, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(response);

        var result = await _fakeController.CallSendValidated(_dispatcherMock.Object, _fakeRequest, CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeAssignableTo<dynamic>().Subject;
        ((bool)value.IsSuccess).Should().BeTrue();
    }

    [Fact]
    public async Task SendValidated_Should_Return_BadRequest_When_IsSuccess_False()
    {
        var response = new FakeResponse
        {
            IsSuccess = false,
            Error = "CustomError",
            Message = "Something went wrong"
        };

        _dispatcherMock.Setup(d =>
            d.SendValidatedAsync<FakeRequest, FakeCommand, FakeResponse>(_fakeRequest, It.IsAny<CancellationToken>())
        ).ReturnsAsync(response);

        var result = await _fakeController.CallSendValidated(_dispatcherMock.Object, _fakeRequest, CancellationToken.None);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorResponse = badRequest.Value.Should().BeOfType<ApiResponse>().Subject;

        errorResponse.Success.Should().BeFalse();
        errorResponse.Errors.Should().ContainSingle(e =>
            e.Error == "CustomError" && e.Detail == "Something went wrong"
        );
    }

    [Fact]
    public async Task SendValidated_Should_Return_BadRequest_When_ValidationException_Thrown()
    {
        
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Field", "Field is required")
        };

        _dispatcherMock.Setup(d =>
            d.SendValidatedAsync<FakeRequest, FakeCommand, FakeResponse>(_fakeRequest, It.IsAny<CancellationToken>())
        ).Throws(new ValidationException(validationFailures));

        var result = await _fakeController.CallSendValidated(_dispatcherMock.Object, _fakeRequest, CancellationToken.None);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errorResponse = badRequest.Value.Should().BeOfType<ApiResponse>().Subject;

        errorResponse.Success.Should().BeFalse();
        errorResponse.Message.Should().Be("Validation failed");
        errorResponse.Errors.Should().ContainSingle(e =>
             e.Detail == "Field is required"
        );
    }

    [Fact]
    public async Task SendValidated_Should_Return_500_When_Unhandled_Exception_Occurs()
    {
       
        _dispatcherMock.Setup(d =>
            d.SendValidatedAsync<FakeRequest, FakeCommand, FakeResponse>(_fakeRequest, It.IsAny<CancellationToken>())
        ).Throws(new Exception("Unexpected error"));

        var result = await _fakeController.CallSendValidated(_dispatcherMock.Object, _fakeRequest, CancellationToken.None);

        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);

        var errorResponse = objectResult.Value.Should().BeOfType<ApiResponse>().Subject;
        errorResponse.Success.Should().BeFalse();
        errorResponse.Errors.Should().ContainSingle(e =>
            e.Error == "UnhandledException" && e.Detail == "Unexpected error"
        );
    }
}
