using TaskManager.Core.Enumerations;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Service;

public class TaskService : ITaskService
{
    private readonly IAppData _appData;
    public TaskService(IAppData appData) => _appData = appData;

    public async Task<long> Add(Task task)
    {
        long id = await this._appData.Tasks.Add(task);

        return id;
    }

    public async Task<long> Assign(Assigment data)
    {
        long id = await this._appData.Tasks.Assign(data);

        return id;
    }

    public async Task<bool> ChangeState(long id, State state)
    {
        bool result = await this._appData.Tasks.ChangeState(id, state);

        return result;
    }

    public async Task<bool> Delete(long id)
    {
        bool result = await this._appData.Tasks.Delete(id);

        if(result) await this._appData.Tasks.DeleteAssigments(id);

        return result;
    }

    public async Task<bool> DeleteAssigment(Assigment assigment)
    {
        bool result = await this._appData.Tasks.DeleteAssigment(assigment);

        return result;
    }

    public async Task<TaskView> Get(long id)
    {
        TaskView result = await this._appData.Tasks.GetView(id);

        return result;
    }

    public async Task<TableResponse<TaskView>> Get(long id, Table table)
    {
        TableResponse<TaskView> result = new TableResponse<TaskView>();

        IEnumerable<TaskView> tasks = await this._appData.Tasks.Get(id, table);

        if (tasks != null && tasks.Count() > 0)
        {
            long total = await this._appData.Tasks.Total(id, table);

            result.Items = tasks.ToList();
            result.Total = total;
        }

        return result;
    }

    public async Task<bool> Update(Task task)
    {
        bool result = await this._appData.Tasks.Update(task);

        return result;
    }

    public async Task<TableResponse<TaskView>> UserTasks(long id, Table table)
    {
        TableResponse<TaskView> result = new TableResponse<TaskView>();

        IEnumerable<TaskView> tasks = await this._appData.Tasks.UserTasks(id, table);

        if (tasks != null && tasks.Count() > 0)
        {
            long total = await this._appData.Tasks.UserTasksTotal(id, table);

            result.Items = tasks.ToList();
            result.Total = total;
        }

        return result;
    }
}

