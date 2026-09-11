using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventManagement.Core.Interfaces;
using EventManagementProject.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EventManagementProject.Controllers;

// Kontroler za registraciju i prijavu korisnika te generiranje JWT tokena
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _authService = authService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        _logger.LogInformation("Pokusaj registracije korisnika: {Email}", dto.Email);

        var user = await _authService.RegisterAsync(dto.Username, dto.Email, dto.Password);
        if (user == null)
        {
            _logger.LogWarning("Registracija neuspjela - email vec postoji: {Email}", dto.Email);
            return BadRequest(new { Message = "Korisnik s tim emailom vec postoji." });
        }

        var roles = await _authService.GetUserRolesAsync(user.Id);
        var token = GenerateJwtToken(user.Id, user.Username, user.Email, roles);

        _logger.LogInformation("Korisnik uspjesno registriran: {Username}", user.Username);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Roles = roles
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        _logger.LogInformation("Pokusaj prijave: {Email}", dto.Email);

        var user = await _authService.LoginAsync(dto.Email, dto.Password);
        if (user == null)
        {
            _logger.LogWarning("Prijava neuspjela za: {Email}", dto.Email);
            return Unauthorized(new { Message = "Neispravni podaci za prijavu." });
        }

        var roles = await _authService.GetUserRolesAsync(user.Id);
        var token = GenerateJwtToken(user.Id, user.Username, user.Email, roles);

        _logger.LogInformation("Korisnik uspjesno prijavljen: {Username}", user.Username);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Roles = roles
        });
    }

    private string GenerateJwtToken(int userId, string username, string email, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
