using Enterprise.Shared.Database.PostgreSql.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Enterprise.Shared.Database.PostgreSql;

public abstract class DbContextBase<TDbContext>(DbContextOptions<TDbContext> options, CustomDbContextOptions<TDbContext> customDbContextOptions)
    : RelationalDbContextBase<TDbContext>(options, customDbContextOptions)
    where TDbContext : DbContextBase<TDbContext>
{
    protected override void ConfigureProviderModel(ModelBuilder builder)
    {
        if (CustomDbContextOptions.IsPostgisEnabled)
        {
            builder.HasPostgresExtension("postgis");
        }
    }

    protected override IInterceptor CreateSelectForUpdateCommandInterceptor() => new SelectForUpdateCommandInterceptor();
}
