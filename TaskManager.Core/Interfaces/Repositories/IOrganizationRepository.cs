using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Repositories;

public interface IOrganizationRepository:IBaseRepository<Organization>
{
    long Exists(string tag, long id = 0);
}
