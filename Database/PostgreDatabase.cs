using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Migrator.Database.Abstraction;

namespace Migrator.Database;

public class PostgreDatabase : Database<NpgsqlConnection>
{
    public override NpgsqlConnection Connect(IConfigurationSection configurationSection)
    {
        var connection = new NpgsqlConnection(GetConnectionString(configurationSection));
        connection.Open();
        Connection = connection;
        return connection;
    }

    public override DataTable GetTable(string tableName)
    {
        using (var command = new NpgsqlCommand("SELECT * FROM " + tableName, Connection))
        {
            var reader = command.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }
    }

    public override void ExecuteQuery(string query)
    {
        using (var command = new NpgsqlCommand(query, Connection))
        {
            command.ExecuteNonQuery();
        }
    }

    public override string GetConfigurationName()
    {
        return "postgresql";
    }
}