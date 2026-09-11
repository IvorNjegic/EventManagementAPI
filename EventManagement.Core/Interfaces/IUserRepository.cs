using EventManagement.Core.Models;

namespace EventManagement.Core.Interfaces;

// Sučelje repozitorija korisnika - proširuje generički repozitorij dodatnim upitima
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<List<string>> GetUserRolesAsync(int userId);
    Task AssignRoleAsync(int userId, int roleId);
    Task<bool> AssignRoleByNameAsync(int userId, string roleName);
    Task<List<User>> GetAllWithRolesAsync();
}
