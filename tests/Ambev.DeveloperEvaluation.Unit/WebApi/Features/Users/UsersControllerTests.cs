using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Features.Users;
public class UsersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _controller = new UsersController(_mediatorMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateUser_Returns_Created_When_Valid()
    {
        var request = new CreateUserRequest
        {
            Username = "John Doe",
            Email = "john@example.com",
            Password = "P@ssword123",
            Phone = "+1234567890",
            Status = UserStatus.Active,
            Role = UserRole.Manager
        };

        var command = new CreateUserCommand();
        var result = new CreateUserResponse();

        _mapperMock.Setup(x => x.Map<CreateUserCommand>(request)).Returns(command);
        _mediatorMock.Setup(x => x.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(new CreateUserResult());
        _mapperMock.Setup(x => x.Map<CreateUserResponse>(It.IsAny<CreateUserResult>())).Returns(result);

        var response = await _controller.CreateUser(request, CancellationToken.None);

        response.Should().BeOfType<CreatedResult>();

        var createdResult = response as CreatedResult;
        createdResult!.Value.Should().BeOfType<ApiResponseWithData<CreateUserResponse>>();
        var apiResponse = (ApiResponseWithData<CreateUserResponse>)createdResult.Value!;
        apiResponse.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetUser_Returns_Ok_When_Valid()
    {
        var userId = Guid.NewGuid();

        var getUserRequest = new GetUserRequest { Id = userId };
        var getUserCommand = new GetUserCommand(userId);
        var getUserResult = new GetUserResult();
        var getUserResponse = new GetUserResponse();

        _mapperMock.Setup(x => x.Map<GetUserCommand>(userId)).Returns(getUserCommand);
        _mediatorMock.Setup(x => x.Send(getUserCommand, It.IsAny<CancellationToken>())).ReturnsAsync(getUserResult);
        _mapperMock.Setup(x => x.Map<GetUserResponse>(getUserResult)).Returns(getUserResponse);

        var result = await _controller.GetUser(userId, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value.Should().BeOfType<ApiResponseWithData<ApiResponseWithData<GetUserResponse>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Message.Should().Be("User retrieved successfully");
        response.Data.Data.Should().BeSameAs(getUserResponse);
    }

    [Fact]
    public async Task DeleteUser_Returns_Ok_When_Valid()
    {
        var id = Guid.NewGuid();
        var command = new DeleteUserCommand(id);

        _mapperMock.Setup(x => x.Map<DeleteUserCommand>(id)).Returns(command);
        _mediatorMock.Setup(x => x.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(new DeleteUserResponse());

        var response = await _controller.DeleteUser(id, CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();

        var okResult = response as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponseWithData<ApiResponse>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data!.Message.Should().Be("User deleted successfully");
    }

    [Fact]
    public async Task CreateUser_Returns_BadRequest_If_Invalid()
    {
        var request = new CreateUserRequest
        {
            Username = "",
            Email = ""
        };

        var controller = new UsersController(_mediatorMock.Object, _mapperMock.Object);

        var result = await controller.CreateUser(request, CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();

        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().BeAssignableTo<IEnumerable<ValidationFailure>>();
        ((IEnumerable<ValidationFailure>)badRequest.Value!).Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUser_Returns_BadRequest_When_Validation_Fails()
    {
        var id = Guid.Empty;
        var controller = new UsersController(_mediatorMock.Object, _mapperMock.Object);

        var result = await controller.GetUser(id, CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();

        var badRequest = result as BadRequestObjectResult;
        badRequest!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        badRequest.Value.Should().NotBeNull();
        badRequest.Value.Should().BeAssignableTo<IEnumerable<ValidationFailure>>();
    }

    [Fact]
    public async Task DeleteUser_Returns_BadRequest_When_Validation_Fails()
    {
        var id = Guid.Empty;
        var controller = new UsersController(_mediatorMock.Object, _mapperMock.Object);

        var result = await controller.DeleteUser(id, CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();

        var badRequest = result as BadRequestObjectResult;
        badRequest!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequest.Value.Should().NotBeNull();
        badRequest.Value.Should().BeAssignableTo<IEnumerable<ValidationFailure>>();
    }
}