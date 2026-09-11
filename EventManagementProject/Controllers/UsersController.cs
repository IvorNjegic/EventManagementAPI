using EventManagement.Core.Interfaces;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserRepository userRepository, IAuthService authService, ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        _logger.LogInformation("Admin dohvaca sve korisnike");
        var users = await _userRepository.GetAllWithRolesAsync();

        var response = users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            CreatedAt = u.CreatedAt,
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
        });

        return Ok(response);
    }

    [HttpPost("{userId}/assign-role")]
    public async Task<IActionResult> AssignRole(int userId, [FromBody] string roleName)
    {
        _logger.LogInformation("Dodjela uloge {Role} korisniku {UserId}", roleName, userId);

        var result = await _authService.AssignRoleAsync(userId, roleName);
        if (!result)
            return BadRequest(new { Message = "Uloga nije pronadjena ili korisnik ne postoji." });

        _logger.LogInformation("Uloga {Role} dodijeljena korisniku {UserId}", roleName, userId);
        return Ok(new { Message = "Uloga uspjesno dodijeljena." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Brisanje korisnika: {UserId}", id);
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { Message = "Korisnik nije pronadjen." });

        await _userRepository.DeleteAsync(id);
        _logger.LogInformation("Korisnik obrisan: {UserId}", id);
        return NoContent();
    }
}
