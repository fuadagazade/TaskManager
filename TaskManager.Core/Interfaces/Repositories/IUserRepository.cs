using TaskManager.Core.Enumerations;
using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    long Exists(string email, string tag, long userId = 0);
    long Exists(string email, long organizationId, long userId = 0);
    Task<bool> DeleteByOrganization(long id);
    Task<IEnumerable<User>> GetByOrganization(long id);
    Task<bool> UpdatePassword(long userId, string password);
    Task<bool> UpdateRole(long userId, Role role);
    Task<User> Identity(Authentication authentication);
}
