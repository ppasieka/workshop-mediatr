using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace QuizService.Tests;

public class QuizAppFactory : WebApplicationFactory<Startup>, IAsyncLifetime
{
    private readonly DbConnection _connection = Database.GetConnection(useInMemory: true);
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDbConnection));
            services.RemoveAll(typeof(DbConnection));
            services.AddSingleton<IDbConnection>(_connection);
            services.AddSingleton(_connection);
        });
    }

    public async Task InitializeAsync()
    {
        await Database.RunMigration(_connection);
    }

    public new Task DisposeAsync()
    {
        _connection.Dispose();
        return Task.CompletedTask;
    }
}