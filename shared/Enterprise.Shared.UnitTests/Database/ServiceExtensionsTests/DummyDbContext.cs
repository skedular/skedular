using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.UnitTests.Database.ServiceExtensionsTests;

public class DummyDbContext : DbContext
{
    public DummyDbContext(
        DbContextOptions<DummyDbContext> options) : base(options)
    {
    }
}
