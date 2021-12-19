using Dapper;
using System.Data;
using System.Data.SqlClient;
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

    public Task<bool> Delete(long id)
    {
        throw new NotImplementedException();
    }

    public long Exists(string email, string tag)
    {
        string command = @"SELECT U.Id FROM Users AS U INNER JOIN Organizations AS O ON U.OrganizationId = O.Id WHERE U.Email = @Email AND O.Tag = @Tag";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Email", email);
        parameters.Add("@Tag", tag);


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

    public Task<IEnumerable<User>> Get()
    {
        throw new NotImplementedException();
    }

    public Task<User> Get(long id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(User user)
    {
        throw new NotImplementedException();
    }
}