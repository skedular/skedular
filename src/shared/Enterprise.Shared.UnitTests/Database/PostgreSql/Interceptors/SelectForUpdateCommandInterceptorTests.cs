using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Database.PostgreSql.Interceptors;
using Npgsql;

namespace Enterprise.Shared.UnitTests.Database.PostgreSql.Interceptors;

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

    [Theory]
    [AutoFakeItEasyData]
    public void ScalarExecuting_adds_for_update(SelectForUpdateCommandInterceptor sut)
    {
        var command = new NpgsqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT COUNT(*) FROM widgets");

        sut.ScalarExecuting(command, null!, default);

        command.CommandText.ShouldEndWith("FOR UPDATE");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ReaderExecutingAsync_adds_for_update(SelectForUpdateCommandInterceptor sut, CancellationToken cancellationToken)
    {
        var command = new NpgsqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT * FROM widgets");

        await sut.ReaderExecutingAsync(command, null!, default, cancellationToken);

        command.CommandText.ShouldEndWith("FOR UPDATE");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ScalarExecutingAsync_adds_for_update(SelectForUpdateCommandInterceptor sut, CancellationToken cancellationToken)
    {
        var command = new NpgsqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT COUNT(*) FROM widgets");

        await sut.ScalarExecutingAsync(command, null!, default, cancellationToken);

        command.CommandText.ShouldEndWith("FOR UPDATE");
    }
}
