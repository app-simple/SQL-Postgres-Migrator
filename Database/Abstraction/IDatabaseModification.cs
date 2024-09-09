using System.Data;

namespace Migrator.Database.Abstraction;

public interface IDatabaseModification
{ 
    /// <summary>
    /// Retrieves the specified table from the database.
    /// </summary>
    /// <param name="tableName">The name of the table to retrieve.</param>
    /// <returns>The data table containing the table data.</returns>
    DataTable GetTable(string tableName);

    /// <summary>
    /// Executes the specified query on the database.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    void ExecuteQuery(string query);
}