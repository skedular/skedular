namespace Enterprise.Shared.Database;

public class CustomDbContextOptions
{
    public bool IsPooled { get; set; }
    public bool IsPostgisEnabled { get; set; }
}

public class CustomDbContextOptions<TDbContext> : CustomDbContextOptions where TDbContext : class;
