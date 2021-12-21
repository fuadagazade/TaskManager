using TaskManager.Core.Enumerations;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;

namespace TaskManager.Core.Interfaces.Services;

public interface IUserService
{
    Task<long> Add(User user);
    long Exists(string email, string tag, long userId = 0);
    long Exists(string email, long organizationId, long userId = 0);
    bool Exists(long id);
    Task<bool> Delete(long id);
    Task<User> Get(long id);
    Task<IEnumerable<User>> Get();
    Task<IEnumerable<User>> GetByOrganization(long id);
    Task<TableResponse<User>> GetByOrganization(long id, Table table);
    Task<bool> Update(User user);
    Task<bool> UpdatePassword(long userId, string oldPassword, string password);
    Task<bool> UpdateRole(long userId, Role role);
}

