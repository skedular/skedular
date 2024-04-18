using System.Collections.Concurrent;
using Npgsql;

namespace Enterprise.Shared.Database;

public static class PostgreSqlConfigurationExtensions
{
    private static readonly ConcurrentDictionary<string, NpgsqlDataSource> s_dataSources = new();

    internal static NpgsqlDataSource BuildDataSource(this PostgresConfigurationOptions configuration)
    {
        try
        {
            if (s_dataSources.TryGetValue(configuration.DefaultConnection, out var dataSource))
            {
                return dataSource;
            }

            s_dataSources[configuration.DefaultConnection] =
                new NpgsqlDataSourceBuilder(configuration.DefaultConnection)
                    .EnableDynamicJson()
                    .Build();
            return s_dataSources[configuration.DefaultConnection];
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Failed to build npgsql datasource {message}", ex.Message);
        }
    }
}
