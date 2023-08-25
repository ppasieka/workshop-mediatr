using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace QuizService.Tests;

public class QuizAppFactory : WebApplicationFactory<Startup>
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        DbConnection connection = Database.GetConnection();
        Database.RunMigration(connection);
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDbConnection));
            services.RemoveAll(typeof(DbConnection));
            services.AddSingleton<IDbConnection>(connection);
            services.AddSingleton(connection);
        });
    }
}