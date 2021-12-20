using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services;

public interface IOrganizationService
{
    Task<long> Add(Organization organization);
    long Exists(string tag, long id = 0);

}

