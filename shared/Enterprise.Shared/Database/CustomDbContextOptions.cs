namespace Enterprise.Shared.Database;

public class CustomDbContextOptions
{
    public bool IsPooled { get; set; }
    public bool IsPostgisEnabled { get; set; }
}

public sealed class CustomDbContextOptions<TDbContext> : CustomDbContextOptions where TDbContext : class;
