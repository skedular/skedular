using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Enterprise.Shared.Database;

/// <summary>
///     Migration Helper used by the Shared Domain projects
/// </summary>
/// <remarks>
///     <see cref="Console.WriteLine()" /> is used instead of logging as Logging might not be available at the time of an
///     exception
/// </remarks>
public static class MigrationHelper
{
    private static readonly string? s_assemblyName = Assembly.GetExecutingAssembly().FullName;

    public static async Task RunMigrationAsync<TDbContext>(Func<IHostBuilder> func, CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        Console.WriteLine($"### {s_assemblyName} -- START");

        var (host, scope) = GetHostServiceScope(func);

        using (host)
        {
            using (scope)
            {
                await MigrateDatabaseAsync<TDbContext>(scope, cancellationToken);
            }
        }

        Console.WriteLine($"### {s_assemblyName} -- END");
    }

    private static (IHost host, IServiceScope scope) GetHostServiceScope(
        Func<IHostBuilder> hostBuilderFunc)
    {
        try
        {
            var host = hostBuilderFunc().Build();
            var scope = host.Services.CreateScope();

            return (host, scope);
        }
        catch (Exception ex)
        {
            LogException(ex, "Failed on Host Build");

            throw;
        }
    }

    private static async Task MigrateDatabaseAsync<TDbContext>(IServiceScope scope, CancellationToken cancellationToken)
        where TDbContext : DbContext
    {
        try
        {
            var scopedServiceProvider = scope.ServiceProvider;
            var dbContextFactory = scopedServiceProvider.GetRequiredService<IDbContextFactory<TDbContext>>();

            await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var dbConnection = dbContext.Database.GetDbConnection();
            var connectionString = dbConnection.ConnectionString;
            var connectionStringParts = connectionString.Split(";", StringSplitOptions.RemoveEmptyEntries);
            var serverPart = connectionStringParts.FirstOrDefault(part => part.StartsWith("Server=", StringComparison.InvariantCultureIgnoreCase));

            Console.WriteLine($"### {s_assemblyName} -- MIGRATING. {serverPart} - Database: {dbConnection.Database}");
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            LogException(ex, "Failed on migration");

            throw;
        }
    }

    private static void LogException(Exception exception, string message)
    {
        Console.WriteLine($"### {s_assemblyName} -- EXCEPTION START");
        Console.WriteLine(message);
        Console.WriteLine(exception);
        Console.WriteLine("### EXCEPTION END");
    }
}
