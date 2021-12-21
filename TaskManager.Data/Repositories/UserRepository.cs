using Dapper;
using System.Data;
using System.Data.SqlClient;
using TaskManager.Core.Enumerations;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;

namespace TaskManager.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SqlConnection context;

    public UserRepository() => context = DataContext.GetConnection();

    public async Task<long> Add(User user)
    {
        string command = @"INSERT INTO Users(FirstName, LastName, Email, Password, Image, Role, OrganizationId, Approved, Status) 
                            OUTPUT INSERTED.ID 
                            VALUES(@FirstName, @LastName, @Email, @Password, @Image, @Role, @OrganizationId, @Approved, @Status)";

        long result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteScalarAsync<long>(command, user);
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

    public async Task<bool> Delete(long id)
    {
        string command = @"UPDATE Users SET STATUS = -1 WHERE Id = @Id";

        int result = 0;

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);

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

    public async Task<bool> DeleteByOrganization(long id)
    {
        string command = @"UPDATE Users SET STATUS = -1 WHERE Organization = @Id";

        int result = 0;

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);

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

    public long Exists(string email, string tag, long userId = 0)
    {
        string command = @"SELECT U.Id FROM Users AS U INNER JOIN Organizations AS O ON U.OrganizationId = O.Id WHERE U.Email = @Email AND O.Tag = @Tag AND U.Id <> @UserId";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Email", email);
        parameters.Add("@Tag", tag);
        parameters.Add("@UserId", userId);

        long result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = connection.ExecuteScalar<long>(command, parameters);
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

    public long Exists(string email, long organizationId, long userId = 0)
    {
        string command = @"SELECT Id FROM Users WHERE Email = @Email AND OrganizationId = @OrganizationId AND Id <> @UserId";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Email", email);
        parameters.Add("@OrganizationId", organizationId);
        parameters.Add("@UserId", userId);

        long result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = connection.ExecuteScalar<long>(command, parameters);
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

    public async Task<IEnumerable<User>> Get()
    {
        string command = @"SELECT * FROM Users WHERE Status > 0";

        IEnumerable<User> result = null;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryAsync<User>(command);
            }
            catch (Exception ex)
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

    public async Task<IEnumerable<User>> GetByOrganization(long id)
    {
        string command = @"SELECT * FROM Users WHERE Status > 0 AND OrganizationId = @Id";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        IEnumerable<User> result = null;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryAsync<User>(command, parameters);
            }
            catch (Exception ex)
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

    public async Task<User> Get(long id)
    {
        string command = @"SELECT * FROM Users WHERE Status > 0 AND Id = @Id";

        User result = null;

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", id);

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryFirstOrDefaultAsync<User>(command, parameters);
            }
            catch (Exception ex)
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

    public async Task<bool> Update(User user)
    {
        string command = @"UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Password = @Password, Image = @Image, Role = @Role, OrganizationId = @OrganizationId, Approved = @Approved, Status = @Status WHERE Id = @Id";

        int result = 0;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteAsync(command, user);
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

    public async Task<bool> UpdatePassword(long userId, string password)
    {
        string command = @"UPDATE Users SET Password = @Password WHERE Id = @Id";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Password", password);
        parameters.Add("@Id", userId);

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

    public async Task<bool> UpdateRole(long userId, Role role)
    {
        string command = @"UPDATE Users SET Role = @Role WHERE Id = @Id";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Role", role);
        parameters.Add("@Id", userId);

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

    public async Task<User> Identity(Authentication authentication)
    {
        string command = @"SELECT U.* FROM Users AS U INNER JOIN Organizations AS O ON U.OrganizationId = O.Id WHERE U.Email = @Email AND O.Tag = @Tag AND U.Password = @Password";

        User result = null;

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Email", authentication.Email);
        parameters.Add("@Tag", authentication.Organization);
        parameters.Add("@Password", authentication.Encrypted);

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryFirstOrDefaultAsync<User>(command, parameters);
            }
            catch (Exception ex)
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

    public async Task<IEnumerable<User>> GetByOrganization(long id, Table table)
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
            "firstName" => "FirstName",
            "lastName" => "LastName",
            "email" => "Email",
            "status" => "Status",
            _ => "Id"
        };

        string search = $@"AND (UPPER(FirstName) LIKE UPPER(@SEARCH) OR UPPER(LastName) LIKE UPPER(@SEARCH) OR Email LIKE LOWER(@SEARCH))";

        string command = $@"SELECT Id, 
                                    FirstName, 
                                    LastName, 
                                    Email,
                                    [Status] 
                                    FROM Users
                                    WHERE 1=1 {(hasStatus ? "AND Status = @STATUS" : "")} {(hasSearch ? search : "")}
                                    ORDER BY {orderBy} {(direction == "desc" ? "DESC" : "ASC")}
                                    {(page != -1 ? "OFFSET @OFFSET ROWS FETCH NEXT @FETCH ROWS ONLY" : "")}";

        IEnumerable<User> result = null;

        DynamicParameters parameters = new DynamicParameters();

        if (hasStatus) parameters.Add(@"STATUS", status);
        if (hasSearch) parameters.Add(@"SEARCH", $"%{searchTerm}%");

        if (page != -1)
        {
            parameters.Add(@"OFFSET", page * size);
            parameters.Add(@"FETCH", size);
        }

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryAsync<User>(command, parameters);
            }
            catch (Exception e)
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

        string search = $@"AND (UPPER(Name) LIKE UPPER(@SEARCH) OR Tag LIKE UPPER(@SEARCH))";

        string command = $@"SELECT COUNT(Id) 
                            FROM Users
                            WHERE 1=1 {(hasStatus ? "AND Status = @STATUS" : "")} {(hasSearch ? search : "")}";

        long result = 0;

        DynamicParameters parameters = new DynamicParameters();

        if (hasStatus) parameters.Add(@"STATUS", status);
        if (hasSearch) parameters.Add(@"SEARCH", $"%{searchTerm}%");

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.ExecuteScalarAsync<long>(command, parameters);
            }
            catch (Exception e)
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

    public async Task<string> GetPassword(long userId)
    {
        string command = @"SELECT Password FROM Users WHERE Status > 0 AND Id = @Id";

        string result = "";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Id", userId);

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryFirstOrDefaultAsync<string>(command, parameters);
            }
            catch (Exception ex)
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
}