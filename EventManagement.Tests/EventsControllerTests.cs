using System.Security.Claims;
using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;
using EventManagementProject.Controllers;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EventManagement.Tests;

public class EventsControllerTests
{
    private readonly Mock<IEventRepository> _mockEventRepo;
    private readonly Mock<ILogger<EventsController>> _mockLogger;
    private readonly EventsController _controller;

    public EventsControllerTests()
    {
        _mockEventRepo = new Mock<IEventRepository>();
        _mockLogger = new Mock<ILogger<EventsController>>();
        _controller = new EventsController(_mockEventRepo.Object, _mockLogger.Object);
    }

    private void SetupUserClaims(int userId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithEvents()
    {
        // Arrange
        var events = new List<Event>
        {
            new Event
            {
                Id = 1, Title = "Test Event",
                Category = new EventCategory { Name = "Konferencija" },
                Venue = new Venue { Name = "Arena" },
                Organizer = new User { Username = "admin" },
                Registrations = new List<Registration>()
            }
        };
        _mockEventRepo.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(events);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEvents = Assert.IsAssignableFrom<IEnumerable<EventResponseDto>>(okResult.Value);
        Assert.Single(returnedEvents);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenEventExists()
    {
        // Arrange
        var ev = new Event
        {
            Id = 1, Title = "Test Event",
            Category = new EventCategory { Name = "Konferencija" },
            Venue = new Venue { Name = "Arena" },
            Organizer = new User { Username = "admin" },
            Registrations = new List<Registration>(),
            Comments = new List<Comment>()
        };
        _mockEventRepo.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ev);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<EventResponseDto>(okResult.Value);
        Assert.Equal("Test Event", response.Title);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        _mockEventRepo.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((Event?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenValid()
    {
        // Arrange
        SetupUserClaims(1, "Admin");
        var dto = new EventCreateDto
        {
            Title = "Novi Event", Description = "Opis",
            StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddHours(2),
            MaxParticipants = 100, CategoryId = 1, VenueId = 1
        };
        var createdEvent = new Event
        {
            Id = 1, Title = "Novi Event",
            Category = new EventCategory { Name = "Konferencija" },
            Venue = new Venue { Name = "Arena" },
            Organizer = new User { Username = "admin" },
            Registrations = new List<Registration>()
        };

        _mockEventRepo.Setup(r => r.AddAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);
        _mockEventRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(createdEvent);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenEventExists()
    {
        // Arrange
        var ev = new Event { Id = 1, Title = "Test" };
        _mockEventRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ev);
        _mockEventRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        _mockEventRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Event?)null);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
