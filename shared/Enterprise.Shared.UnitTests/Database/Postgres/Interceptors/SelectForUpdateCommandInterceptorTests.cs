using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Database.Postgres.Interceptors;
using Npgsql;

namespace Enterprise.Shared.UnitTests.Database.Postgres.Interceptors;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SelectForUpdateCommandInterceptorTests
{
    [Theory]
    [AutoFakeItEasyData]
    public void Add_for_update_for_matching_queries(SelectForUpdateCommandInterceptor sut)
    {
        var command = new NpgsqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT * FROM widgets");

        sut.ReaderExecuting(command, null!, default);

        command.CommandText.ShouldEndWith("FOR UPDATE");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Add_for_update_skip_locked_for_matching_queries(SelectForUpdateCommandInterceptor sut)
    {
        var command = new NpgsqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdateSkipLocked}{Environment.NewLine}SELECT * FROM widgets");

        sut.ReaderExecuting(command, null!, default);

        command.CommandText.ShouldEndWith("FOR UPDATE SKIP LOCKED");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Leave_non_matching_queries_unchanged(SelectForUpdateCommandInterceptor sut)
    {
        var command = new NpgsqlCommand("SELECT * FROM widgets");

        sut.ReaderExecuting(command, null!, default);

        command.CommandText.ShouldBe("SELECT * FROM widgets");
    }
}
