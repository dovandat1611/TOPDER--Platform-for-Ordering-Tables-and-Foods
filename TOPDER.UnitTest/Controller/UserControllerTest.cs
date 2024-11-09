using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.User;
using TOPDER.Service.IServices;
using TOPDER.API.Controllers;
using TOPDER.Repository.Entities;
using System;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Utils;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<JwtHelper> _mockJwtHelper;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        // Initialize required mocks
        _mockUserService = new Mock<IUserService>();
        _mockJwtHelper = new Mock<JwtHelper>(/* pass any required dependencies here if needed */);

        // Initialize UserController with mocked dependencies
        _controller = new UserController(
            null,                // _restaurantService
            null,                // _cloudinaryService
            null,                // _sendMailService
            _mockUserService.Object, // _userService
            null,                // _customerService
            null,                // _walletService
            _mockJwtHelper.Object, // _jwtHelper
            null,                // _adminService
            null,                // _userRepository
            null,                // _identityService
            null                 // _userOtpRepository
        );
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WithRestaurantInfo_OnSuccessfulLogin()
    {
        // Arrange
        var loginModel = new LoginModel { Email = "restaurant@example.com", Password = "password123" };
        var userLoginDto = new UserLoginDTO
        {
            Uid = Guid.NewGuid(),
            Email = loginModel.Email,
            Role = User_Role.RESTAURANT
        };

        _mockUserService.Setup(s => s.GetUserByEmailAndPassword(loginModel))
            .ReturnsAsync(userLoginDto);

        _mockJwtHelper.Setup(j => j.GenerateJwtToken(userLoginDto))
            .Returns("generated-jwt-token");

        // Act
        var result = await _controller.Login(loginModel) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var response = result.Value as ApiResponse;
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal("Authentication success.", response.Message);
        Assert.NotNull(response.Data);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _controller.Login(new LoginModel()) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Dữ liệu không hợp lệ.", result.Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenUnauthorizedAccessExceptionThrown()
    {
        // Arrange
        var loginModel = new LoginModel { Email = "user@example.com", Password = "wrongpassword" };
        _mockUserService.Setup(s => s.GetUserByEmailAndPassword(loginModel))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

        // Act
        var result = await _controller.Login(loginModel) as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
        Assert.Equal("Invalid credentials", (result.Value as dynamic).message);
    }

    [Fact]
    public async Task Login_ReturnsServerError_OnUnhandledException()
    {
        // Arrange
        var loginModel = new LoginModel { Email = "user@example.com", Password = "password123" };
        _mockUserService.Setup(s => s.GetUserByEmailAndPassword(loginModel))
            .ThrowsAsync(new Exception("Something went wrong"));

        // Act
        var result = await _controller.Login(loginModel) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
        Assert.Equal("Có lỗi xảy ra. Vui lòng thử lại sau.", (result.Value as dynamic).message);
    }
}
