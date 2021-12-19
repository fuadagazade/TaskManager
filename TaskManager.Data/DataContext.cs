using Azersun.Audit.Utilities;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace TaskManager.Data;

public static class DataContext
{
    private static IConfigurationSection ConnectionStrings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings");

    public static SqlConnection GetConnection()
    {
        string ConnectionString = ConnectionStrings["Main"];

        string connection = Cryptographer.Decrypt(ConnectionString);

        return new SqlConnection(connection);
    }

    public static void GenerateDatabase()
    {
        using (SqlConnection connection = GetConnection())
        {
            DatabaseGenerator.Install(connection);
        }
    }
}