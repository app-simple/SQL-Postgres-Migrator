using System.Data;
using Migrator.Converters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace Migrator.Database.Abstraction;

/// <summary>
/// Abstract base class for database operations.
/// </summary>
/// <typeparam name="T">The type of the database connection.</typeparam>
public abstract class Database<T> : IDatabaseModification where T : IDbConnection
{
    /// <summary>
    /// Gets or sets the database connection.
    /// </summary>
    protected T Connection { get; set; }
    
    /// <summary>
    /// Retrieves the connection string from the configuration section.
    /// </summary>
    /// <param name="configurationSection">The configuration section containing the connection string.</param>
    /// <returns>The connection string.</returns>
    /// <exception cref="InvalidConfigurationException">Thrown when the configuration is missing or the connection string is null.</exception>
    protected string GetConnectionString(IConfigurationSection configurationSection)
    {
        // Retrieve the configuration section for the database.
        IConfigurationSection section = configurationSection.GetSection(GetConfigurationName());
        if (section == null)
        {
            throw new InvalidConfigurationException("Configuration is missing database type " + GetConfigurationName());
        }
        
        // Retrieve the connection string from the configuration section.
        string? connectionString = section["connectionString"];
        if (connectionString == null)
        {
            throw new InvalidConfigurationException("Connection string is null");
        }
        
        return connectionString;
    }
    
    /// <summary>
    /// Connects to the database using the provided configuration section.
    /// </summary>
    /// <param name="configurationSection">The configuration section containing the connection details.</param>
    /// <returns>The database connection.</returns>
    public abstract T Connect(IConfigurationSection configurationSection);

    /// <summary>
    /// Gets the name of the configuration section for the database.
    /// </summary>
    /// <returns>The name of the configuration section.</returns>
    public abstract string GetConfigurationName();
    
    /// <summary>
    /// Transfers tables from the current database to the target database.
    /// </summary>
    /// <param name="targetDatabase">The target database to transfer tables to.</param>
    /// <param name="converter">The converter to use for data conversion.</param>
    /// <param name="tables">The list of tables to transfer.</param>
    public void TransferTableToTargetDatabase(IDatabaseModification targetDatabase, IConverter converter, string[] tables)
    {
        // Loop through each table and transfer the data to the target database.
        foreach (string table in tables)
        {
            // Retrieve the table data.
            DataTable dataTable = this.GetTable(table);
            // Loop through each row in the table and insert it into the target database.
            foreach (DataRow row in dataTable.Rows)
            {
                Console.WriteLine(converter.ConvertDataRowToInsertStatement(dataTable, row, table));
                try
                {
                    // Execute the through the IConverter generated query on the target database.
                    targetDatabase.ExecuteQuery(converter.ConvertDataRowToInsertStatement(dataTable, row, table));
                }
                catch (Exception exception)
                {
                    // Don't!! Stop on duplicate error, there will be duplicate and it's fine
                    if (exception.Message.Contains("duplicate"))
                    {
                        continue;
                    }
                    
                    // Otherwise print the error message.
                    Console.WriteLine("Error: " + exception.Message);
                }
            }
        }
    }

    /// <summary>
    /// Retrieves the specified table from the database.
    /// </summary>
    /// <param name="tableName">The name of the table to retrieve.</param>
    /// <returns>The data table containing the table data.</returns>
    public virtual DataTable GetTable(string tableName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Executes the specified query on the database.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    public virtual void ExecuteQuery(string query)
    {
        throw new NotImplementedException();
    }
}