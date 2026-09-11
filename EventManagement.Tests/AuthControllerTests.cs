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

    // Uspješna registracija treba vratiti Ok odgovor s tokenom i podacima korisnika
    [Fact]
    public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
    {
        // Priprema
        var dto = new RegisterDto { Username = "testuser", Email = "test@test.com", Password = "Test123!" };
        var user = new User { Id = 1, Username = "testuser", Email = "test@test.com" };

        _mockAuthService.Setup(s => s.RegisterAsync(dto.Username, dto.Email, dto.Password))
            .ReturnsAsync(user);
        _mockAuthService.Setup(s => s.GetUserRolesAsync(user.Id))
            .ReturnsAsync(new List<string> { "User" });

        // Izvršavanje
        var result = await _controller.Register(dto);

        // Provjera
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("testuser", response.Username);
        Assert.Equal("test@test.com", response.Email);
        Assert.Contains("User", response.Roles);
        Assert.False(string.IsNullOrEmpty(response.Token));
    }

    // Ako email već postoji, registracija treba vratiti BadRequest
    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        // Priprema
        var dto = new RegisterDto { Username = "testuser", Email = "test@test.com", Password = "Test123!" };

        _mockAuthService.Setup(s => s.RegisterAsync(dto.Username, dto.Email, dto.Password))
            .ReturnsAsync((User?)null);

        // Izvršavanje
        var result = await _controller.Register(dto);

        // Provjera
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Ispravni podaci za prijavu trebaju vratiti Ok s generiranim tokenom
    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsAreValid()
    {
        // Priprema
        var dto = new LoginDto { Email = "test@test.com", Password = "Test123!" };
        var user = new User { Id = 1, Username = "testuser", Email = "test@test.com" };

        _mockAuthService.Setup(s => s.LoginAsync(dto.Email, dto.Password))
            .ReturnsAsync(user);
        _mockAuthService.Setup(s => s.GetUserRolesAsync(user.Id))
            .ReturnsAsync(new List<string> { "User" });

        // Izvršavanje
        var result = await _controller.Login(dto);

        // Provjera
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("testuser", response.Username);
        Assert.False(string.IsNullOrEmpty(response.Token));
    }

    // Neispravni podaci za prijavu trebaju vratiti Unauthorized
    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        // Priprema
        var dto = new LoginDto { Email = "test@test.com", Password = "wrong" };

        _mockAuthService.Setup(s => s.LoginAsync(dto.Email, dto.Password))
            .ReturnsAsync((User?)null);

        // Izvršavanje
        var result = await _controller.Login(dto);

        // Provjera
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }
}
