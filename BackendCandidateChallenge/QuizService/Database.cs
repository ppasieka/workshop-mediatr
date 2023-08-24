using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace QuizService;

internal static class Database
{
    public static DbConnection GetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        return connection;
    }

    public static void RunMigration(DbConnection connection)
    {
        var assembly = typeof(IAssemblyMarker).GetTypeInfo().Assembly;
        var migrationResourceNames = assembly
            .GetManifestResourceNames()
            .Where(x => x.EndsWith(".sql"))
            .OrderBy(x => x)
            .ToArray();
        if (migrationResourceNames.Length == 0)
        {
            throw new System.Exception("No migration files found!");
        }
        
        foreach (var resourceName in migrationResourceNames)
        {
            var sql = GetResourceText(assembly, resourceName);
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }

    private static string GetResourceText(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new System.Exception($"Resource {resourceName} not found!");
        }
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}