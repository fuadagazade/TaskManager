using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;

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

    public async Task<IEnumerable<Organization>> Get()
    {
        IEnumerable<Organization> organizations = await this._appData.Organizations.Get();

        return organizations;
    }

    public long Exists(string tag, long id = 0)
    {
        long organizationId = this._appData.Organizations.Exists(tag, id);

        return organizationId;
    }

    public async Task<bool> Delete(long id)
    {
        bool result = await this._appData.Organizations.Delete(id);

        if(result) await this._appData.Users.DeleteByOrganization(id);

        return result;
    }

    public async Task<Organization> Get(long id)
    {
        Organization organization = await this._appData.Organizations.Get(id);

        return organization;
    }

    public async Task<bool> Update(OrganizationUpdate data)
    {
        bool result = await this._appData.Organizations.Update(data);

        return result;
    }

    public async Task<TableResponse<Organization>> Get(Table table)
    {
        TableResponse<Organization> result = new TableResponse<Organization>();

        IEnumerable<Organization> organizations = await this._appData.Organizations.Get(table);

        if (organizations != null && organizations.Count() > 0)
        {
            long total = await this._appData.Organizations.Total(table);

            result.Items = organizations.ToList();
            result.Total = total;
        }

        return result;
    }

    public bool Exists(long id)
    {
        bool result = this._appData.Organizations.Exists(id);

        return result;
    }
}