using Dapper;
using System.Data;
using System.Data.SqlClient;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Models;

namespace TaskManager.Data.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly SqlConnection context;

    public OrganizationRepository()=> context = DataContext.GetConnection();

    public async Task<long> Add(Organization data)
    {
        string command = @"INSERT INTO Organizations(Name, Tag, Phone, Address, Logo, Status) 
                            OUTPUT INSERTED.ID 
                            VALUES(@Name, @Tag, @Phone, @Address, @Logo, @Status)";

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

    public async Task<bool> Delete(long id)
    {
        string command = @"UPDATE Organizations SET STATUS = -1 WHERE Id = @id";

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

    public long Exists(string tag, long id = 0)
    {
        string command = @"SELECT Id FROM Organizations WHERE Tag = @Tag AND Id <> @Id";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Tag", tag);
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
                return 0;
            }
            finally
            {
                connection.Dispose();
            }
        }

        return result;
    }

    public async Task<IEnumerable<Organization>> Get()
    {
        string command = @"SELECT * FROM Organizations WHERE Status > 0";

        IEnumerable<Organization> result = null;

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryAsync<Organization>(command);
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

    public async Task<Organization> Get(long id)
    {
        string command = @"SELECT * FROM Organizations WHERE Status > 0 AND Id = @id";

        Organization result = null;

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@id", id);

        using (IDbConnection connection = context)
        {
            try
            {
                result = await connection.QueryFirstOrDefaultAsync<Organization>(command, parameters);
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

    public async Task<bool> Update(Organization data)
    {
        string command = @"UPDATE Organizations SET Name = @Name, Tag = @Tag, Phone = @Phone, Address = @Address, Logo = @Logo, Status = @Status WHERE Id = @id";

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
}