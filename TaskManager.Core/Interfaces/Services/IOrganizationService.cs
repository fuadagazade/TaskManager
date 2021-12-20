using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;

namespace TaskManager.Core.Interfaces.Services;

public interface IOrganizationService
{
    Task<long> Add(Organization organization);
    Task<IEnumerable<Organization>> Get();
    long Exists(string tag, long id = 0);
    Task<bool> Delete(long id);
    Task<Organization> Get(long id);
    Task<bool> Update(OrganizationUpdate data);
    Task<TableResponse<Organization>> Get(Table table);
}

