using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services;

public interface ITokenService
{
    string Create(User user);
}

