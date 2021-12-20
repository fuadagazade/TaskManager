using Azersun.Audit.Utilities;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly IAppData _appData;
    public AuthenticationService(IAppData appData) => _appData = appData;

    public async Task<User> Authenticate(Authentication authentication)
    {
        return await this._appData.Users.Identity(authentication);
    }
}
