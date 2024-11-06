using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.UnitTests.Database.ServiceExtensionsTests;

public class DummyDbContext(DbContextOptions<DummyDbContext> options) : DbContext(options);
