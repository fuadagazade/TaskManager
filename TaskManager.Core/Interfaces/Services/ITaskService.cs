using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Core.Interfaces.Services;

public interface ITaskService
{
    Task<long> Add(Task task);
}

