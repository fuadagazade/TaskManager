using TaskManager.Core.Enumerations;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;

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

    public async Task<bool> Delete(long id)
    {
        bool result = await this._appData.Users.Delete(id);

        return result;
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

    public bool Exists(long id)
    {
        bool result = this._appData.Users.Exists(id);

        return result;
    }

    public async Task<User> Get(long id)
    {
        User user = await this._appData.Users.Get(id);

        return user;
    }

    public async Task<IEnumerable<User>> Get()
    {
        IEnumerable<User> users = await this._appData.Users.Get();

        return users;
    }

    public async Task<IEnumerable<User>> GetByOrganization(long id)
    {
        IEnumerable<User> users = await this._appData.Users.GetByOrganization(id);

        return users;
    }

    public async Task<TableResponse<User>> GetByOrganization(long id, Table table)
    {
        TableResponse<User> result = new TableResponse<User>();

        IEnumerable<User> users = await this._appData.Users.GetByOrganization(id, table);

        if (users != null && users.Count() > 0)
        {
            long total = await this._appData.Users.Total(id, table);

            result.Items = users.ToList();
            result.Total = total;
        }

        return result;
    }

    public async Task<bool> Update(User user)
    {
        bool result = await this._appData.Users.Update(user);

        return result;
    }

    public async Task<bool> UpdatePassword(long userId, string oldPassword, string password)
    {
        string current = await this._appData.Users.GetPassword(userId);

        if(current != oldPassword) return false;

        bool result = await this._appData.Users.UpdatePassword(userId,password);

        return result;
    }

    public async Task<bool> UpdateRole(long userId, Role role)
    {
        bool result = await this._appData.Users.UpdateRole(userId, role);

        return result;
    }
}