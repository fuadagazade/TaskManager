using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;

namespace TaskManager.Core.Interfaces.Repositories;

public interface IOrganizationRepository:IBaseRepository<Organization>
{
    long Exists(string tag, long id = 0);
    Task<bool> Update(OrganizationUpdate data);
    Task<IEnumerable<Organization>> Get(Table table);
    Task<long> Total(Table table);
}
