using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services;

public interface IAuthenticationService
{
    Task<User> Authenticate(Authentication authentication);
}
