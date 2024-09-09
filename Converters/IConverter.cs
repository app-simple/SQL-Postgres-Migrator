using System.Data;

namespace Migrator.Utility;

public interface IConverter
{
    string ConvertDataRowToInsertStatement(DataTable dataTable, DataRow row, string tableName);
}