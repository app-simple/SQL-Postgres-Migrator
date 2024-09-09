using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Migrator.Database.Abstraction;

namespace Migrator.Database;

public class MsSqlDatabase : Database<SqlConnection>
{
    public override SqlConnection Connect(IConfigurationSection configurationSection)
    {
        var connection = new SqlConnection(GetConnectionString(configurationSection));
        connection.Open();
        Connection = connection;
        return connection;
    }

    public override DataTable GetTable(string tableName)
    {
        using (var command = new SqlCommand("SELECT * FROM " + tableName, Connection))
        {
            var reader = command.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }
    }

    public override void ExecuteQuery(string query)
    {
        using (var command = new SqlCommand(query, Connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public override string GetConfigurationName()
    {
        return "sqlserver";
    }
}