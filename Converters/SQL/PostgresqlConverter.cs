using System.Data;
using Migrator.Utility;

namespace Migrator.Converters.SQL;

public class PostgresqlConverter : IConverter
{
    public string ConvertDataRowToInsertStatement(DataTable dataTable, DataRow row, string tableName)
    {
        List<string> columnNames = new List<string>();
        List<string> values = new List<string>();

        foreach (DataColumn column in dataTable.Columns)
        {
            columnNames.Add("\"" + column.ColumnName + "\"");
            values.Add(FormatValue(row[column]));
        }

        string columns = string.Join(", ", columnNames);
        string valuesString = string.Join(", ", values);
        string insertStatement = $"INSERT INTO \"{tableName}\"({columns}) VALUES({valuesString});";
        return insertStatement;
    }
    
    private string FormatValue(object value)
    {
        if (value == null || value == DBNull.Value)
        {
            return "NULL";
        }
        else if (value is string || value is Guid)
        {
            return $"'{value.ToString().Replace("'", "''")}'";
        }
        else if (value is DateTimeOffset offset)
        {
            return $"'${offset.ToUniversalTime()}'";
        }
        else if (value is DateTime)
        {
            return $"'${value}'";
        }
        else if (value is Byte[])
        {
            return "NULL";
        }
        else
        {
            return value.ToString();
        }
    }
}