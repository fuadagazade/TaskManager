using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Service;

public class OrganizationService : IOrganizationService
{
    private readonly IAppData _appData;
    public OrganizationService(IAppData appData) => _appData = appData;

    public async Task<long> Add(Organization organization)
    {
        long organizationId = await this._appData.Organizations.Add(organization);

        return organizationId;
    }

    public long Exists(string tag, long id = 0)
    {
        long organizationId = this._appData.Organizations.Exists(tag, id);

        return organizationId;
    }
}