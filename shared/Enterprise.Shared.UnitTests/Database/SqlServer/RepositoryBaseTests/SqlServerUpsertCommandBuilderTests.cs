using Enterprise.Shared.Database.SqlServer;

namespace Enterprise.Shared.UnitTests.Database.SqlServer.RepositoryBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SqlServerUpsertCommandBuilderTests
{
    [Fact]
    public void BuildInsertIfMissing_Should_use_not_exists_with_locking_for_primary_key_check()
    {
        var sql = SqlServerUpsertCommandBuilder.BuildInsertIfMissing("[dbo].[Widgets]", "Id", "CreatedAt");

        sql.ShouldContain("INSERT INTO [dbo].[Widgets] ([Id], [CreatedAt])");
        sql.ShouldContain("SELECT @Id, @CreatedAt");
        sql.ShouldContain("FROM [dbo].[Widgets] WITH (UPDLOCK, HOLDLOCK)");
        sql.ShouldContain("WHERE [Id] = @Id");
        sql.ShouldNotContain("MERGE INTO");
    }

    [Fact]
    public void BuildInsertIfMissing_Should_include_foreign_key_columns_when_provided()
    {
        var sql = SqlServerUpsertCommandBuilder.BuildInsertIfMissing("[custom].[Widgets]", "Id", "CreatedAt", "ParentId", "OwnerId");

        sql.ShouldContain("INSERT INTO [custom].[Widgets] ([Id], [CreatedAt], [ParentId], [OwnerId])");
        sql.ShouldContain("SELECT @Id, @CreatedAt, @ParentId, @OwnerId");
    }
}
