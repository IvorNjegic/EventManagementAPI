using EventManagement.Core.Models;

namespace EventManagement.Core.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(string username, string email, string password);
    Task<User?> LoginAsync(string email, string password);
    Task<List<string>> GetUserRolesAsync(int userId);
    Task<bool> AssignRoleAsync(int userId, string roleName);
}
