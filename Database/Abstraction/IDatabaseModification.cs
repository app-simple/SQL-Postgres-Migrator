using System.Data;

namespace Migrator.Database.Abstraction;

public interface IDatabaseModification
{ 
    DataTable GetTable(string tableName);

    void ExecuteQuery(string query);
}