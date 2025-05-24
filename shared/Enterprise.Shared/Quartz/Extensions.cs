using Enterprise.Shared.Database;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Quartz.Impl.AdoJobStore.Common;

namespace Enterprise.Shared.Quartz;

public static class Extensions
{
    public static IServiceCollection WithQuartzNpgsqlDbProvider(this IServiceCollection services, NpgsqlDataSource dataSource) =>
        services.AddSingleton<IDbProvider>(new QuartzNpgsqlDbProvider(dataSource));
}
