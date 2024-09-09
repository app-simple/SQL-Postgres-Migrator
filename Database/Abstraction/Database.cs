using System.Data;
using Migrator.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace Migrator.Database.Abstraction;

public abstract class Database<T> : IDatabaseModification where T : IDbConnection
{
    protected T Connection { get; set; }
    
    protected string GetConnectionString(IConfigurationSection configurationSection)
    {
        IConfigurationSection section = configurationSection.GetSection(GetConfigurationName());
        if (section == null)
        {
            throw new InvalidConfigurationException("Configuration is missing database type " + GetConfigurationName());
        }
        
        string? connectionString = section["connectionString"];
        if (connectionString == null)
        {
            throw new InvalidConfigurationException("Connection string is null");
        }
        
        return connectionString;
    }
    
    public abstract T Connect(IConfigurationSection configurationSection);

    public abstract string GetConfigurationName();
    
    public void TransferTableToTargetDatabase(IDatabaseModification targetDatabase, IConverter converter, string[] tables)
    {
        foreach (string table in tables)
        {
            DataTable dataTable = this.GetTable(table);
            foreach (DataRow row in dataTable.Rows)
            {
                Console.WriteLine(converter.ConvertDataRowToInsertStatement(dataTable, row, table));
                try
                {
                    targetDatabase.ExecuteQuery(converter.ConvertDataRowToInsertStatement(dataTable, row, table));
                }
                catch (Exception exception)
                {
                    if (exception.Message.Contains("duplicate"))
                    {
                        continue;
                    }
                    
                    Console.WriteLine("Error: " + exception.Message);
                }
            }
        }
    }

    public virtual DataTable GetTable(string tableName)
    {
        throw new NotImplementedException();
    }

    public virtual void ExecuteQuery(string query)
    {
        throw new NotImplementedException();
    }
}