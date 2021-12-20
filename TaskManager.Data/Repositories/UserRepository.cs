using Dapper;
using System.Data;
using System.Data.SqlClient;
using TaskManager.Core.Enumerations;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Models;

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
}