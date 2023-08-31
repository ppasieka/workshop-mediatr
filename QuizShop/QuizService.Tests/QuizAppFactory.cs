using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace QuizService.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class QuizAppFactory : WebApplicationFactory<Startup>, IAsyncLifetime
{
    public const string QuizApiEndPoint = "/api/quizzes/";
    private readonly DbConnection _connection = Database.GetConnection(useInMemory: true);

    protected override IHostBuilder CreateHostBuilder()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.ConfigureServices(services =>
                {
                    services.AddSingleton<IDbConnection>(_connection);
                    services.AddSingleton(_connection);
                });
            });
        
        return builder;
    }

    public async Task InitializeAsync()
    {
        Database.RegisterCustomTypeHandlers();
        await Database.RunMigration(_connection);
        await Database.SeedDatabase(_connection);
    }

    public new Task DisposeAsync()
    {
        _connection.Dispose();
        return Task.CompletedTask;
    }
}