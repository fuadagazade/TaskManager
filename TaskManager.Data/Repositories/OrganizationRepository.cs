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

    public Task<bool> Delete(long id)
    {
        throw new NotImplementedException();
    }

    public long Exists(string tag)
    {
        string command = @"SELECT Id FROM Organizations WHERE Tag = @Tag";

        DynamicParameters parameters = new DynamicParameters();
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

    public Task<IEnumerable<Organization>> Get()
    {
        throw new NotImplementedException();
    }

    public Task<Organization> Get(long id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(Organization data)
    {
        throw new NotImplementedException();
    }
}