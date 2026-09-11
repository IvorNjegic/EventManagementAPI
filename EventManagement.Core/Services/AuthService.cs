using EventManagement.Core.Interfaces;
using EventManagement.Core.Models;

namespace EventManagement.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> RegisterAsync(string username, string email, string password)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
            return null;

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.AssignRoleAsync(user.Id, 2);

        return user;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<List<string>> GetUserRolesAsync(int userId)
    {
        return await _userRepository.GetUserRolesAsync(userId);
    }

    public async Task<bool> AssignRoleAsync(int userId, string roleName)
    {
        return await _userRepository.AssignRoleByNameAsync(userId, roleName);
    }
}
