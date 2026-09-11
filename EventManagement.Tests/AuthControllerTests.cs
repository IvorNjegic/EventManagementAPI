using System.Security.Claims;
using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;
using EventManagementProject.Controllers;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EventManagement.Tests;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockConfig = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<AuthController>>();

        _mockConfig.Setup(c => c["Jwt:Key"]).Returns("TvojSuperTajniKljucKojiMoraImatiBarem32Znaka!!");
        _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("EventManagementAPI");
        _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("EventManagementClient");

        _controller = new AuthController(_mockAuthService.Object, _mockConfig.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
    {
        // Arrange
        var dto = new RegisterDto { Username = "testuser", Email = "test@test.com", Password = "Test123!" };
        var user = new User { Id = 1, Username = "testuser", Email = "test@test.com" };

        _mockAuthService.Setup(s => s.RegisterAsync(dto.Username, dto.Email, dto.Password))
            .ReturnsAsync(user);
        _mockAuthService.Setup(s => s.GetUserRolesAsync(user.Id))
            .ReturnsAsync(new List<string> { "User" });

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("testuser", response.Username);
        Assert.Equal("test@test.com", response.Email);
        Assert.Contains("User", response.Roles);
        Assert.False(string.IsNullOrEmpty(response.Token));
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var dto = new RegisterDto { Username = "testuser", Email = "test@test.com", Password = "Test123!" };

        _mockAuthService.Setup(s => s.RegisterAsync(dto.Username, dto.Email, dto.Password))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Register(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsAreValid()
    {
        // Arrange
        var dto = new LoginDto { Email = "test@test.com", Password = "Test123!" };
        var user = new User { Id = 1, Username = "testuser", Email = "test@test.com" };

        _mockAuthService.Setup(s => s.LoginAsync(dto.Email, dto.Password))
            .ReturnsAsync(user);
        _mockAuthService.Setup(s => s.GetUserRolesAsync(user.Id))
            .ReturnsAsync(new List<string> { "User" });

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("testuser", response.Username);
        Assert.False(string.IsNullOrEmpty(response.Token));
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var dto = new LoginDto { Email = "test@test.com", Password = "wrong" };

        _mockAuthService.Setup(s => s.LoginAsync(dto.Email, dto.Password))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }
}
