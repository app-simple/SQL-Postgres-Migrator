using Migrator.Converters.SQL;
using Migrator.Database.Abstraction;
using Migrator.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npgsql;

IConfigurationRoot configurationRoot = new ConfigurationBuilder()
    // Set the base path for the configuration builder to the application's base directory
    .SetBasePath(AppContext.BaseDirectory)
    // Add the JSON configuration file "appsettings.json" and enable reloading on change
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Retrieve the "databases" section from the configuration
IConfigurationSection databases = configurationRoot.GetSection("databases");

string[] GetTables()
{
    // Split the comma-separated list of tables from the configuration
    return databases["tablesToTransfer"]!.Split(",");
}

// Create and connect to the source database (SQL Server)
Database<SqlConnection> sourceDatabase = new MsSqlDatabase();
sourceDatabase.Connect(databases);

// Create and connect to the target database (PostgreSQL)
Database<NpgsqlConnection> targetDatabase = new PostgreDatabase();
targetDatabase.Connect(databases);

// Transfer tables from the source database to the target database using a PostgresqlConverter
sourceDatabase.TransferTableToTargetDatabase(targetDatabase, new PostgresqlConverter(), GetTables());