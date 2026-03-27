using Enterprise.Shared.Database.Postgres;

namespace Enterprise.Shared.UnitTests.Database.Postgres.RepositoryBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class PostgresUpsertCommandBuilderTests
{
    [Fact]
    public void BuildInsertIfMissing_Should_use_on_conflict_do_nothing()
    {
        var sql = PostgresUpsertCommandBuilder.BuildInsertIfMissing("public.\"widgets\"", "Id", "CreatedAt");

        sql.ShouldContain("INSERT INTO public.\"widgets\" (\"Id\", \"CreatedAt\")");
        sql.ShouldContain("VALUES (@Id, @CreatedAt)");
        sql.ShouldContain("ON CONFLICT (\"Id\") DO NOTHING");
    }

    [Fact]
    public void BuildInsertIfMissing_Should_include_foreign_key_columns()
    {
        var sql = PostgresUpsertCommandBuilder.BuildInsertIfMissing("custom.\"widgets\"", "Id", "CreatedAt", "ParentId");

        sql.ShouldContain("(\"Id\", \"CreatedAt\", \"ParentId\")");
        sql.ShouldContain("VALUES (@Id, @CreatedAt, @ParentId)");
    }
}
