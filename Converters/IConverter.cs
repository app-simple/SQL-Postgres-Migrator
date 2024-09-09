using System.Data;

namespace Migrator.Converters;

public interface IConverter
{
    /// <summary>
    /// Converts a DataRow to an SQL insert statement.
    /// </summary>
    /// <param name="dataTable">The DataTable containing the data.</param>
    /// <param name="row">The DataRow to convert.</param>
    /// <param name="tableName">The name of the table to insert into.</param>
    /// <returns>A string containing the SQL insert statement.</returns>
    string ConvertDataRowToInsertStatement(DataTable dataTable, DataRow row, string tableName);
}