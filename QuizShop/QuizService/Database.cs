using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using QuizService.Application;

namespace QuizService;

internal static class Database
{
    public static DbConnection GetConnection(bool useInMemory)
    {
        var connection = useInMemory ? GetInMemoryConnection() : GetFileConnection();
        return connection;
    }
    private static DbConnection GetInMemoryConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        return connection;
    }
    
    private static DbConnection GetFileConnection()
    {
        var connection = new SqliteConnection("Data Source=quiz.db");
        connection.Open();
        return connection;
    }

    public static async Task RunMigration(DbConnection connection)
    {
        await CreateMigrationTable(connection);
        foreach (var resourceName in GetMigrationsFromResources())
        {
            await ApplyDatabaseMigrations(connection, resourceName, typeof(IAssemblyMarker).GetTypeInfo().Assembly);
        }
    }

    private static string[] GetMigrationsFromResources()
    {
        var migrationResourceNames = typeof(IAssemblyMarker).GetTypeInfo().Assembly
            .GetManifestResourceNames()
            .Where(x => x.EndsWith(".sql"))
            .OrderBy(x => x)
            .ToArray();
        if (migrationResourceNames.Length == 0)
        {
            throw new Exception("No migration files found!");
        }

        return migrationResourceNames;
    }

    private static async Task ApplyDatabaseMigrations(DbConnection connection, string resourceName, Assembly assembly)
    {
        // Check if migration is applied
        var checkCmd = connection.CreateCommand();
        checkCmd.CommandText = $"SELECT COUNT(*) FROM MigrationHistory WHERE MigrationId = '{resourceName}'";
        var count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

        if (count != 0)
            return;
        
        await ApplyMigration(connection, resourceName, assembly);
        // Record migration in MigrationHistory table
        await UpdateMigrationTable(connection, resourceName);
    }

    private static async Task ApplyMigration(DbConnection connection, string resourceName, Assembly assembly)
    {
        await using var migrationCmd = connection.CreateCommand();
        migrationCmd.CommandText = await GetResourceText(assembly, resourceName);
        await migrationCmd.ExecuteNonQueryAsync();
    }

    private static async Task UpdateMigrationTable(DbConnection connection, string resourceName)
    {
        await using var insertMigrationCmd = connection.CreateCommand();
        insertMigrationCmd.CommandText = $"INSERT INTO MigrationHistory (MigrationId) VALUES ('{resourceName}')";
        await insertMigrationCmd.ExecuteNonQueryAsync();
    }

    private static async Task CreateMigrationTable(DbConnection connection)
    {
        string createTableQuery =
            "CREATE TABLE IF NOT EXISTS MigrationHistory (MigrationId TEXT PRIMARY KEY, AppliedOn DATETIME DEFAULT CURRENT_TIMESTAMP)";
        await using var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = createTableQuery;
        await createTableCmd.ExecuteNonQueryAsync();
    }

    public static void RegisterCustomTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new QuizIdTypeHandler());
    }

    private static async Task<string> GetResourceText(Assembly assembly, string resourceName)
    {
        await using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new Exception($"Resource {resourceName} not found!");
        }
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}