using Dapper;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DapperRepositories.NpgSql;

public static class DatabaseManager
{
    public static void EnsureDatabase(string connectionString, string dbName)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        
        var originalDbName = builder.Database;
        if (string.IsNullOrEmpty(originalDbName))
            originalDbName = dbName;

        builder.Database = "postgres";
        
        using var connection = new NpgsqlConnection(builder.ConnectionString);
        connection.Open();

        var exists = connection.ExecuteScalar<bool>(
            "SELECT 1 FROM pg_database WHERE datname = @name", 
            new { name = originalDbName });

        if (!exists)
        {
            connection.Execute($"CREATE DATABASE \"{originalDbName}\"");
        }
    }

    public static void MigrateUp(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}