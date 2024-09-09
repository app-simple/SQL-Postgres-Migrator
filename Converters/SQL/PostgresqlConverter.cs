using System.Data;
using Migrator.Converters;

namespace Migrator.Converters.SQL;

public class PostgresqlConverter : IConverter
{
    public string ConvertDataRowToInsertStatement(DataTable dataTable, DataRow row, string tableName)
    {
        // Create a list of column names and values
        List<string> columnNames = new List<string>();
        List<string> values = new List<string>();

        // Add each column name and it's row value to the lists
        foreach (DataColumn column in dataTable.Columns)
        {
            columnNames.Add("\"" + column.ColumnName + "\"");
            values.Add(FormatValue(row[column]));
        }

        // Create the insert statement
        string columns = string.Join(", ", columnNames);
        string valuesString = string.Join(", ", values);
        string insertStatement = $"INSERT INTO \"{tableName}\"({columns}) VALUES({valuesString});";
        return insertStatement;
    }
    
    private string FormatValue(object value)
    {
        // Check if the value is null or DBNull
        if (value == null || value == DBNull.Value)
        {
            // Return NULL if it is
            return "NULL";
        }
        // Check if the value is a string or Guid
        else if (value is string || value is Guid)
        {
            // Escape single quotes and return the value in single quotes
            return $"'{value.ToString().Replace("'", "''")}'";
        }
        // Check if the value is a DateTimeOffset
        else if (value is DateTimeOffset offset)
        {
            // Return the value in single quotes
            return $"'${offset.ToUniversalTime()}'";
        }
        // Check if the value is a DateTime
        else if (value is DateTime)
        {
            // Return the value in single quotes
            return $"'${value}'";
        }
        // Check if the value is a byte array
        else if (value is byte[])
        {
            // Return NULL if it is because we don't really support byte arrays
            return "NULL";
        }
        else
        {
            // Otherwise just return the object's value. In case of a number, boolean or other primitive types this should be enough.
            // If not, add another else statement to handle the type.
            return value.ToString();
        }
    }
}