using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Data.Repositories;


namespace TaskManager.Data;

public class AppData : IAppData
{
    private IOrganizationRepository organizations;
    private IUserRepository users;
    private ITaskRepository tasks;



    public IOrganizationRepository Organizations => organizations ?? new OrganizationRepository();
    public IUserRepository Users => users ?? new UserRepository();

    public ITaskRepository Tasks => tasks ?? new TaskRepository();

    public AppData()
    {

    }
}
