using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;
using EventManagementProject.Controllers;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EventManagement.Tests;

public class UsersControllerTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ILogger<UsersController>> _mockLogger;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_mockUserRepo.Object, _mockAuthService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User
            {
                Id = 1, Username = "admin", Email = "admin@test.com",
                UserRoles = new List<UserRole>
                {
                    new UserRole { Role = new Role { Name = "Admin" } }
                }
            }
        };
        _mockUserRepo.Setup(r => r.GetAllWithRolesAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<UserResponseDto>>(okResult.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task AssignRole_ReturnsOk_WhenRoleExists()
    {
        // Arrange
        _mockAuthService.Setup(s => s.AssignRoleAsync(1, "Admin")).ReturnsAsync(true);

        // Act
        var result = await _controller.AssignRole(1, "Admin");

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AssignRole_ReturnsBadRequest_WhenRoleDoesNotExist()
    {
        // Arrange
        _mockAuthService.Setup(s => s.AssignRoleAsync(1, "SuperAdmin")).ReturnsAsync(false);

        // Act
        var result = await _controller.AssignRole(1, "SuperAdmin");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
