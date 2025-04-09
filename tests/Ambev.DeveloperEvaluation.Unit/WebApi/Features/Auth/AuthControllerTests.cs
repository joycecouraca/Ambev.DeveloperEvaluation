using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth.AuthenticateUserFeature;
using Ambev.DeveloperEvaluation.WebApi.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using MediatR;
using Moq;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Auth;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _controller = new AuthController(_mediatorMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task AuthenticateUser_Returns_Ok_When_Valid()
    {
        var request = new AuthenticateUserRequest
        {
            Email = "testuser@email.com",
            Password = "P@ssword123"
        };

        var command = new AuthenticateUserCommand();
        var commandResult = new AuthenticateUserResult();

        var expectedResponse = new AuthenticateUserResponse
        {
            Token = "jwt-token",
            Email = request.Email,
            Name = "Test User",
            Role = "Manager"
        };

        _mapperMock.Setup(x => x.Map<AuthenticateUserCommand>(request)).Returns(command);
        _mediatorMock.Setup(x => x.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(commandResult);
        _mapperMock.Setup(x => x.Map<AuthenticateUserResponse>(commandResult))
            .Returns(new AuthenticateUserResponse
            {
                Token = "jwt-token",
                Email = request.Email,
                Name = "Test User",
                Role = "Manager"
            });

        var result = await _controller.AuthenticateUser(request, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;

        okResult!.Value.Should().BeOfType<ApiResponseWithData<ApiResponseWithData<AuthenticateUserResponse>>>();
        var apiResponse = okResult!.Value.Should().BeOfType<ApiResponseWithData<ApiResponseWithData<AuthenticateUserResponse>>>().Subject;

        apiResponse!.Data!.Success.Should().BeTrue();
        apiResponse.Data.Message.Should().Be("User authenticated successfully");
        apiResponse.Data.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task AuthenticateUser_Returns_BadRequest_When_Invalid()
    {
        var invalidRequest = new AuthenticateUserRequest();

        var result = await _controller.AuthenticateUser(invalidRequest, CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;

        badRequest!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequest.Value.Should().BeAssignableTo<IEnumerable<FluentValidation.Results.ValidationFailure>>();
    }
}
