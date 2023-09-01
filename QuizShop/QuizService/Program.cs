using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace QuizService;

public class Program
{
    public static async Task Main(string[] args)
    {
        DbConnection connection = Database.GetConnection(useInMemory: false);
        Database.RegisterCustomTypeHandlers();
        await Database.RunMigration(connection);
        await Database.SeedDatabase(connection);
        IHost host = BuildWebHost(args, connection);
        await host.RunAsync();
    }

    private static IHost BuildWebHost(string[] args, DbConnection connection) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>();
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IDbConnection>(connection);
                    services.AddSingleton(connection);
                });
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build();
}