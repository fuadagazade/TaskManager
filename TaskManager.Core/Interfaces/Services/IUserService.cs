using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services;

public interface IUserService
{
    Task<long> Add(User user);
    long Exists(string email, string tag);
    long Exists(string email, long organizationId);
}

