using TaskManager.Core.Enumerations;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Interfaces.Services;

public interface ITaskService
{
    Task<long> Add(Task task);
    Task<bool> Update(Task task);
    Task<bool> Delete(long id);
    Task<long> Assign(Assigment data);
    Task<bool> DeleteAssigment(Assigment assigment);
    Task<TaskView> Get(long id);
    Task<TableResponse<TaskView>> Get(long id, Table table);
    Task<TableResponse<TaskView>> UserTasks(long id, Table table);
    Task<bool> ChangeState(long id, State state);
}

