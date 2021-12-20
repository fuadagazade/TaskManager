using Dapper;
using System.Data;
using System.Data.SqlClient;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Models;
using TaskManager.Core.Models.Table;
using TaskManager.Core.ViewModels;

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

    public async Task<IEnumerable<Organization>> Get(Table table)
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
            "name" => "Name",
            "tag" => "Tag",
            "status" => "Status",
            _ => "Id"
        };

        string search = $@"AND (UPPER(Name) LIKE UPPER(@SEARCH) OR Tag LIKE UPPER(@SEARCH))";

        string command = $@"SELECT Id, 
                                    Name, 
                                    Tag, 
                                    Phone, 
                                    Address, 
                                    [Status] 
                                    FROM Organizations
                                    WHERE 1=1 {(hasStatus ? "AND STATUS = @STATUS" : "")} {(hasSearch ? search : "")}
                                    ORDER BY {orderBy} {(direction == "desc" ? "DESC" : "ASC")}
                                    {(page != -1 ? "OFFSET @OFFSET ROWS FETCH NEXT @FETCH ROWS ONLY" : "")}";

        IEnumerable<Organization> result = null;

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
                result = await connection.QueryAsync<Organization>(command, parameters);
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

    public async Task<long> Total(Table table)
    {
        bool hasStatus = table.Filter.ContainsKey("status");
        bool hasSearch = !string.IsNullOrEmpty(table.Search);

        int status = hasStatus ? Int32.Parse(table.Filter["status"]) : 1;
        string searchTerm = hasSearch ? table.Search : "";
        string direction = table.Sorting.Direction;
        int page = table.Paginator.Page;
        int size = table.Paginator.Size;

        string search = $@"AND (UPPER(Name) LIKE UPPER(@SEARCH) OR Tag LIKE UPPER(@SEARCH))";

        string command = $@"SELECT COUNT(Id) 
                            FROM Organizations
                            WHERE 1=1 {(hasStatus ? "AND STATUS = @STATUS" : "")} {(hasSearch ? search : "")}";

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

    public async Task<bool> Update(OrganizationUpdate data)
    {
        string command = @"UPDATE Organizations SET Name = @Name, Phone = @Phone, Address = @Address WHERE Id = @id";

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