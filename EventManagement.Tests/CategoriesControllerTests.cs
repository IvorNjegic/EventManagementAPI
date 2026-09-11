using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;
using EventManagementProject.Controllers;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EventManagement.Tests;

public class CategoriesControllerTests
{
    private readonly Mock<IRepository<EventCategory>> _mockRepo;
    private readonly Mock<ILogger<CategoriesController>> _mockLogger;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _mockRepo = new Mock<IRepository<EventCategory>>();
        _mockLogger = new Mock<ILogger<CategoriesController>>();
        _controller = new CategoriesController(_mockRepo.Object, _mockLogger.Object);
    }

    // Dohvat svih kategorija treba vratiti Ok s listom kategorija
    [Fact]
    public async Task GetAll_ReturnsOkWithCategories()
    {
        // Priprema
        var categories = new List<EventCategory>
        {
            new EventCategory { Id = 1, Name = "Konferencija" },
            new EventCategory { Id = 2, Name = "Radionica" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Izvršavanje
        var result = await _controller.GetAll();

        // Provjera
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<CategoryResponseDto>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }

    // Dohvat nepostojeće kategorije treba vratiti NotFound
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Priprema
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((EventCategory?)null);

        // Izvršavanje
        var result = await _controller.GetById(99);

        // Provjera
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // Kreiranje kategorije treba vratiti CreatedAtAction (201)
    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        // Priprema
        var dto = new CategoryCreateDto { Name = "Nova kategorija", Description = "Opis" };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<EventCategory>())).Returns(Task.CompletedTask);

        // Izvršavanje
        var result = await _controller.Create(dto);

        // Provjera
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    // Brisanje postojeće kategorije treba vratiti NoContent
    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        // Priprema
        var category = new EventCategory { Id = 1, Name = "Test" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
        _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Izvršavanje
        var result = await _controller.Delete(1);

        // Provjera
        Assert.IsType<NoContentResult>(result);
    }
}
