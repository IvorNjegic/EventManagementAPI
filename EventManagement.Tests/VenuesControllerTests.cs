using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;
using EventManagementProject.Controllers;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EventManagement.Tests;

public class VenuesControllerTests
{
    private readonly Mock<IRepository<Venue>> _mockRepo;
    private readonly Mock<ILogger<VenuesController>> _mockLogger;
    private readonly VenuesController _controller;

    public VenuesControllerTests()
    {
        _mockRepo = new Mock<IRepository<Venue>>();
        _mockLogger = new Mock<ILogger<VenuesController>>();
        _controller = new VenuesController(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithVenues()
    {
        // Arrange
        var venues = new List<Venue>
        {
            new Venue { Id = 1, Name = "Arena", Address = "Zagreb", Capacity = 500 }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(venues);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<VenueResponseDto>>(okResult.Value);
        Assert.Single(returned);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenVenueDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Venue?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new VenueCreateDto { Name = "Nova lokacija", Address = "Adresa", Capacity = 100 };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Venue>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        // Arrange
        var venue = new Venue { Id = 1, Name = "Test" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(venue);
        _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
