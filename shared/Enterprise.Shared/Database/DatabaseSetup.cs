using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Enterprise.Shared.Database;

public class DatabaseSetup
{
    protected internal DatabaseSetup(
        IServiceCollection serviceCollection,
        NpgsqlDataSource npgsqlDataSource)
    {
        ServiceCollection = serviceCollection;
        NpgsqlDataSource = npgsqlDataSource;
    }

    internal IServiceCollection ServiceCollection { get; }
    internal NpgsqlDataSource NpgsqlDataSource { get; }
}
