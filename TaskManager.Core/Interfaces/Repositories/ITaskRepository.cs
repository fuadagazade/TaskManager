using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Interfaces.Repositories;

public interface ITaskRepository :IBaseRepository<Task>
{
    Task<long> Assign(Assigment data);
    Task<IEnumerable<TaskView>> UserTasks(long id, Table table);
    Task<IEnumerable<TaskView>> Get(long id, Table table);
    Task<long> Total(long id, Table table);
    Task<long> UserTasksTotal(long id, Table table);
    bool Exists(long id);
    Task<bool> DeleteAssigments(long id);
    Task<TaskView> GetView(long id);
    Task<IEnumerable<TaskView>> GetViews(long id);
}

