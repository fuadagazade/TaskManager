using Dapper;
using System.Data;
using System.Data.SqlClient;
using TaskManager.Core.Enumerations;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;
using Task = TaskManager.Core.Models.Task;

namespace TaskManager.Data.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly SqlConnection context;

    public TaskRepository() => context = DataContext.GetConnection();

    public async Task<long> Add(Task data)
    {
        string command = @"INSERT INTO Tasks(Title, Description, Priority, Point, State, Deadline, OrganizationId, CreatorId) 
                            OUTPUT INSERTED.ID 
                            VALUES(@Title, @Description, @Priority, @Point, @State, #Deadline, @OrganizationId, @CreatorId)";

        long result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteScalarAsync<long>(command, data);
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }

    public async Task<long> Assign(Assigment data)
    {
        string command = @"INSERT INTO Assigments(TaskId, UserId) 
                            OUTPUT INSERTED.ID 
                            VALUES(@TaskId, @Description, @UserId)";

        long result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteScalarAsync<long>(command, data);
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }

    public async Task<bool> ChangeState(long id, State state)
    {
        string command = @"UPDATE Tasks SET State = @State WHERE Id = @Id";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);
        parameters.Add("@State", state);


        int result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteAsync(command, parameters);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result > 0;
    }

    public async Task<bool> Delete(long id)
    {
        string command = @"UPDATE Tasks SET STATUS = -1 WHERE Id = @id";

        int result = 0;

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@id", id);

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteAsync(command, parameters);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result > 0;
    }

    public async Task<bool> DeleteAssigment(Assigment assigment)
    {
        string command = @"UPDATE Assigments SET STATUS = -1 WHERE TaskId = @TaskId AND UserId = @UserId";

        int result = 0;

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@TaskId", assigment.TaskId);
        parameters.Add("@UserId", assigment.UserId);

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteAsync(command, parameters);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result > 0;
    }

    public async Task<bool> DeleteAssigments(long id)
    {
        string command = @"UPDATE Assigments SET STATUS = -1 WHERE TaskId = @id";

        int result = 0;

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@id", id);

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteAsync(command, parameters);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result > 0;
    }

    public bool Exists(long id)
    {
        string command = @"SELECT Id FROM Tasks WHERE Id = @Id";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        long result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = connection.ExecuteScalar<long>(command, parameters);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result > 0;
    }

    public async Task<IEnumerable<TaskView>> Get(long id, Table table)
    {
        bool hasStatus = table.Filter.ContainsKey("status");
        bool hasSearch = !string.IsNullOrEmpty(table.Search);

        int status = hasStatus ? Int32.Parse(table.Filter["status"]) : 1;
        string searchTerm = hasSearch ? table.Search : "";
        string direction = table.Sorting.Direction;
        int page = table.Paginator.Page;
        int size = table.Paginator.Size;

        string orderBy = table.Sorting.Column switch
        {
            "id" => "Id",
            "title" => "Title",
            "priority" => "Priority",
            "point" => "Point",
            "state" => "State",
            "deadline" => "Deadline",
            "createDate" => "CreateDate",
            _ => "Id"
        };

        string search = $@"AND (UPPER(Title) LIKE UPPER(@SEARCH) OR UPPER(Description) LIKE UPPER(@SEARCH))";

        string command = $@"SELECT * FROM Tasks
                        WHERE OrganizationId = @Id {(hasStatus ? "AND Status = @STATUS" : "")} {(hasSearch ? search : "")}
        ORDER BY { orderBy} { (direction == "desc" ? "DESC" : "ASC")}
        {(page != -1 ? "OFFSET @OFFSET ROWS FETCH NEXT @FETCH ROWS ONLY" : "")}";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);
        if (hasStatus) parameters.Add(@"STATUS", status);
        if (hasSearch) parameters.Add(@"SEARCH", $"%{searchTerm}%");

        if (page != -1)
        {
            parameters.Add(@"OFFSET", page * size);
            parameters.Add(@"FETCH", size);
        }

        IEnumerable<TaskView> result = null;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryAsync<TaskView>(command, parameters);

                command = @"SELECT U.* 
                                    FROM Users as U INNER JOIN Assigments AS A ON U.Id = A.UserId
                                    WHERE A.Id = @Id";

                foreach (TaskView item in result)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@Id", item.Id);
                    item.Assigments = await connection.QueryAsync<TaskUser>(command, parameters);
                }
            }
            catch (Exception)
            {

                return null;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }

    public async Task<IEnumerable<Task>> Get()
    {
        string command = @"SELECT * FROM Tasks";

        IEnumerable<Task> result = null;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryAsync<Task>(command);
            }
            catch (Exception)
            {

                return null;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }

    public async Task<Task> Get(long id)
    {
        string command = @"SELECT * FROM Tasks WHERE Id = @Id";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        Task result = null;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryFirstOrDefaultAsync<Task>(command, parameters);
            }
            catch (Exception)
            {

                return null;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }

    public async Task<TaskView> GetView(long id)
    {
        string command = @"SELECT * FROM Tasks WHERE Id = @Id";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        TaskView result = null;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryFirstOrDefaultAsync<TaskView>(command, parameters);

                command = @"SELECT U.* 
                                    FROM Users as U INNER JOIN Assigments AS A ON U.Id = A.UserId
                                    WHERE A.Id = @Id";


                parameters = new DynamicParameters();
                parameters.Add("@Id", result.Id);
                result.Assigments = await connection.QueryAsync<TaskUser>(command, parameters);
            }
            catch (Exception)
            {

                return null;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }

    public async Task<IEnumerable<TaskView>> GetViews(long id)
    {
        string command = @"SELECT * FROM Tasks WHERE OrganizationId = @Id";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        IEnumerable<TaskView> result = null;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryAsync<TaskView>(command, parameters);

                command = @"SELECT U.* 
                                    FROM Users as U INNER JOIN Assigments AS A ON U.Id = A.UserId
                                    WHERE A.Id = @Id";

                foreach (TaskView item in result)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@Id", item.Id);
                    item.Assigments = await connection.QueryAsync<TaskUser>(command, parameters);
                }
            }
            catch (Exception)
            {

                return null;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }

    public async Task<long> Total(long id, Table table)
    {
        bool hasStatus = table.Filter.ContainsKey("status");
        bool hasSearch = !string.IsNullOrEmpty(table.Search);

        int status = hasStatus ? Int32.Parse(table.Filter["status"]) : 1;
        string searchTerm = hasSearch ? table.Search : "";

        string search = $@"AND (UPPER(Title) LIKE UPPER(@SEARCH) OR UPPER(Description) LIKE UPPER(@SEARCH))";

        string command = $@"SELECT * FROM Tasks
                        WHERE OrganizationId = @Id {(hasStatus ? "AND Status = @STATUS" : "")} {(hasSearch ? search : "")}";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);
        if (hasStatus) parameters.Add(@"STATUS", status);
        if (hasSearch) parameters.Add(@"SEARCH", $"%{searchTerm}%");

        long result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteScalarAsync<long>(command, parameters);
            }
            catch (Exception)
            {

                return 0;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }

    public async Task<bool> Update(Task data)
    {
        string command = @"UPDATE Tasks SET Title = @Title, Description = @Description, Priority = @Priority, Point = @Point, State = @State, Deadline = @DeadLine WHERE Id = @Id";

        int result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteAsync(command, data);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result > 0;
    }

    public async Task<IEnumerable<TaskView>> UserTasks(long id, Table table)
    {
        bool hasStatus = table.Filter.ContainsKey("status");
        bool hasSearch = !string.IsNullOrEmpty(table.Search);

        int status = hasStatus ? Int32.Parse(table.Filter["status"]) : 1;
        string searchTerm = hasSearch ? table.Search : "";
        string direction = table.Sorting.Direction;
        int page = table.Paginator.Page;
        int size = table.Paginator.Size;

        string orderBy = table.Sorting.Column switch
        {
            "id" => "Id",
            "title" => "Title",
            "priority" => "Priority",
            "point" => "Point",
            "state" => "State",
            "deadline" => "Deadline",
            "createDate" => "CreateDate",
            _ => "Id"
        };

        string search = $@"AND (UPPER(Title) LIKE UPPER(@SEARCH) OR UPPER(Description) LIKE UPPER(@SEARCH))";

        string command = $@"SELECT * FROM Tasks
                        WHERE Id IN (SELECT TaskId FROM Assigments WHERE UserID = @Id AND Status = 1) {(hasStatus ? "AND Status = @STATUS" : "")} {(hasSearch ? search : "")}
        ORDER BY { orderBy} { (direction == "desc" ? "DESC" : "ASC")}
        {(page != -1 ? "OFFSET @OFFSET ROWS FETCH NEXT @FETCH ROWS ONLY" : "")}";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);
        if (hasStatus) parameters.Add(@"STATUS", status);
        if (hasSearch) parameters.Add(@"SEARCH", $"%{searchTerm}%");

        if (page != -1)
        {
            parameters.Add(@"OFFSET", page * size);
            parameters.Add(@"FETCH", size);
        }

        IEnumerable<TaskView> result = null;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryAsync<TaskView>(command, parameters);

                command = @"SELECT U.* 
                                    FROM Users as U INNER JOIN Assigments AS A ON U.Id = A.UserId
                                    WHERE A.Id = @Id";

                foreach (TaskView item in result)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@Id", item.Id);
                    item.Assigments = await connection.QueryAsync<TaskUser>(command, parameters);
                }
            }
            catch (Exception)
            {

                return null;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }

    public async Task<long> UserTasksTotal(long id, Table table)
    {
        bool hasStatus = table.Filter.ContainsKey("status");
        bool hasSearch = !string.IsNullOrEmpty(table.Search);

        int status = hasStatus ? Int32.Parse(table.Filter["status"]) : 1;
        string searchTerm = hasSearch ? table.Search : "";

        string search = $@"AND (UPPER(Title) LIKE UPPER(@SEARCH) OR UPPER(Description) LIKE UPPER(@SEARCH))";

        string command = $@"SELECT * FROM Tasks
                            WHERE Id IN (SELECT TaskId FROM Assigments WHERE UserID = @Id AND Status = 1) {(hasStatus ? "AND Status = @STATUS" : "")} {(hasSearch ? search : "")}";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);
        if (hasStatus) parameters.Add(@"STATUS", status);
        if (hasSearch) parameters.Add(@"SEARCH", $"%{searchTerm}%");

        long result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteScalarAsync<long>(command, parameters);
            }
            catch (Exception)
            {

                return 0;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }
}

