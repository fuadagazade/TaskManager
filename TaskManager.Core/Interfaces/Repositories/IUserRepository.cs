using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    long Exists(string email, string tag);
}
