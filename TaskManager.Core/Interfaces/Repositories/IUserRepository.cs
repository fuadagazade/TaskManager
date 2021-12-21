using TaskManager.Core.Enumerations;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;

namespace TaskManager.Core.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    long Exists(string email, string tag, long userId = 0);
    long Exists(string email, long organizationId, long userId = 0);
    bool Exists(long id);
    Task<bool> DeleteByOrganization(long id);
    Task<IEnumerable<User>> GetByOrganization(long id);
    Task<IEnumerable<User>> GetByOrganization(long id, Table table);
    Task<long> Total(long id, Table table);
    Task<bool> UpdatePassword(long userId, string password);
    Task<bool> UpdateRole(long userId, Role role);
    Task<User> Identity(Authentication authentication);
    Task<string> GetPassword(long userId);
}
