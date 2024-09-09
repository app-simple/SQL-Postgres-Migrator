using Migrator.Converters.SQL;
using Migrator.Database.Abstraction;
using Migrator.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;

IConfigurationRoot configurationRoot = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

IConfigurationSection databases = configurationRoot.GetSection("databases");

string[] GetTables()
{
    return databases["tablesToTransfer"]!.Split(",");
}

Database<SqlConnection> sourceDatabase = new MsSqlDatabase();
sourceDatabase.Connect(databases);

Database<NpgsqlConnection> targetDatabase = new PostgreDatabase();
targetDatabase.Connect(databases);

sourceDatabase.TransferTableToTargetDatabase(targetDatabase, new PostgresqlConverter(), GetTables());