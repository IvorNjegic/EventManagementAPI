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

    // Dohvat svih lokacija treba vratiti Ok s listom lokacija
    [Fact]
    public async Task GetAll_ReturnsOkWithVenues()
    {
        // Priprema
        var venues = new List<Venue>
        {
            new Venue { Id = 1, Name = "Arena", Address = "Zagreb", Capacity = 500 }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(venues);

        // Izvršavanje
        var result = await _controller.GetAll();

        // Provjera
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<VenueResponseDto>>(okResult.Value);
        Assert.Single(returned);
    }

    // Dohvat nepostojeće lokacije treba vratiti NotFound
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenVenueDoesNotExist()
    {
        // Priprema
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Venue?)null);

        // Izvršavanje
        var result = await _controller.GetById(99);

        // Provjera
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Kreiranje lokacije treba vratiti CreatedAtAction (201)
    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        // Priprema
        var dto = new VenueCreateDto { Name = "Nova lokacija", Address = "Adresa", Capacity = 100 };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Venue>())).Returns(Task.CompletedTask);

        // Izvršavanje
        var result = await _controller.Create(dto);

        // Provjera
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    // Brisanje postojeće lokacije treba vratiti NoContent
    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        // Priprema
        var venue = new Venue { Id = 1, Name = "Test" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(venue);
        _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Izvršavanje
        var result = await _controller.Delete(1);

        // Provjera
        Assert.IsType<NoContentResult>(result);
    }
}
