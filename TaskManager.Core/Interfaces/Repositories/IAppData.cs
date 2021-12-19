namespace TaskManager.Core.Interfaces.Repositories;

public interface IAppData
{
    IOrganizationRepository Organizations { get; }
    IUserRepository Users { get; }
}
