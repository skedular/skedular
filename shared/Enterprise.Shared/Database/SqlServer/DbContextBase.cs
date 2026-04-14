using Enterprise.Shared.Database.SqlServer.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Enterprise.Shared.Database.SqlServer;

public abstract class DbContextBase<TDbContext>(DbContextOptions<TDbContext> options, CustomDbContextOptions<TDbContext> customDbContextOptions)
    : RelationalDbContextBase<TDbContext>(options, customDbContextOptions)
    where TDbContext : DbContextBase<TDbContext>
{
    protected override IInterceptor CreateSelectForUpdateCommandInterceptor() => new SelectForUpdateCommandInterceptor();
}
