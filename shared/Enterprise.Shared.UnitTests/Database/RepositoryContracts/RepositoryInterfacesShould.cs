using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Database.SqlServer;

namespace Enterprise.Shared.UnitTests.Database.RepositoryContracts;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RepositoryInterfacesShould
{
    [Fact]
    public void Not_Expose_Query_On_IRepository()
    {
        typeof(IRepository<>).GetMethod("Query").ShouldBeNull();
    }

    [Fact]
    public void Not_Expose_Query_On_Repository_Bases()
    {
        typeof(Enterprise.Shared.Database.PostgreSql.RepositoryBase<,>).GetMethod("Query").ShouldBeNull();
        typeof(Enterprise.Shared.Database.SqlServer.RepositoryBase<,>).GetMethod("Query").ShouldBeNull();
    }
}