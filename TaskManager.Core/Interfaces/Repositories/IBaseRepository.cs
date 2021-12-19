namespace TaskManager.Core.Interfaces.Repositories;

public interface IBaseRepository<T>
{
    Task<IEnumerable<T>> Get();
    Task<T> Get(long id);
    Task<long> Add(T data);
    Task<bool> Update(T data);
    Task<bool> Delete(long id);
}