using Enterprise.Shared.UnitTests.Database.TestSupport;

namespace Enterprise.Shared.UnitTests.Fixtures;

public sealed class DatabaseTestContextFixtureCustomizer : InMemoryDbContextFixtureCustomizer<DatabaseTestContext>;
