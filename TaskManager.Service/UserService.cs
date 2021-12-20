using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Service;

public class UserService : IUserService
{
    private readonly IAppData _appData;
    public UserService(IAppData appData) => _appData = appData;

    public async Task<long> Add(User user)
    {
        long userId = await this._appData.Users.Add(user);

        return userId;
    }

    public long Exists(string email, string tag, long userId = 0)
    {
        long id = this._appData.Users.Exists(email, tag, userId);

        return id;
    }

    public long Exists(string email, long organizationId, long userId = 0)
    {
        long id = this._appData.Users.Exists(email, organizationId, userId);

        return id;
    }
}