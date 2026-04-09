using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Database.SqlServer.Interceptors;
using Microsoft.Data.SqlClient;

namespace Enterprise.Shared.UnitTests.Database.SqlServer.Interceptors;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SelectForUpdateCommandInterceptorTests
{
    [Theory]
    [AutoFakeItEasyData]
    public void Add_update_lock_hints_for_for_update_queries(SelectForUpdateCommandInterceptor sut)
    {
        var command = new SqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT * FROM [dbo].[Widgets] AS [w]");

        sut.ReaderExecuting(command, null!, default);

        command.CommandText.ShouldContain("FROM [dbo].[Widgets] WITH (UPDLOCK, ROWLOCK) AS [w]");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Add_read_past_hints_for_skip_locked_queries(SelectForUpdateCommandInterceptor sut)
    {
        var command = new SqlCommand(
            $"-- {EntityFrameworkInterceptorTags.ForUpdateSkipLocked}{Environment.NewLine}SELECT * FROM [dbo].[Widgets] AS [w]");

        sut.ReaderExecuting(command, null!, default);

        command.CommandText.ShouldContain("FROM [dbo].[Widgets] WITH (UPDLOCK, READPAST, ROWLOCK) AS [w]");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Leave_queries_without_matching_tag_unchanged(SelectForUpdateCommandInterceptor sut)
    {
        var command = new SqlCommand("SELECT * FROM [dbo].[Widgets] AS [w]");

        sut.ReaderExecuting(command, null!, default);

        command.CommandText.ShouldBe("SELECT * FROM [dbo].[Widgets] AS [w]");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ScalarExecuting_adds_lock_hints(SelectForUpdateCommandInterceptor sut)
    {
        var command = new SqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT COUNT(*) FROM [dbo].[Widgets]");

        sut.ScalarExecuting(command, null!, default);

        command.CommandText.ShouldContain("WITH (UPDLOCK, ROWLOCK)");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ReaderExecutingAsync_adds_lock_hints(SelectForUpdateCommandInterceptor sut)
    {
        var command = new SqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT * FROM [dbo].[Widgets] AS [w]");

        await sut.ReaderExecutingAsync(command, null!, default);

        command.CommandText.ShouldContain("WITH (UPDLOCK, ROWLOCK)");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ScalarExecutingAsync_adds_lock_hints(SelectForUpdateCommandInterceptor sut)
    {
        var command = new SqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT COUNT(*) FROM [dbo].[Widgets]");

        await sut.ScalarExecutingAsync(command, null!, default);

        command.CommandText.ShouldContain("WITH (UPDLOCK, ROWLOCK)");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Handle_query_with_no_from_clause(SelectForUpdateCommandInterceptor sut)
    {
        var command = new SqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT 1");

        sut.ReaderExecuting(command, null!, default);

        // No FROM clause — command should be unchanged (no crash)
        command.CommandText.ShouldContain("SELECT 1");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Handle_query_with_comma_separated_tables(SelectForUpdateCommandInterceptor sut)
    {
        var command = new SqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT * FROM [dbo].[Widgets],[dbo].[Items]");

        sut.ReaderExecuting(command, null!, default);

        command.CommandText.ShouldContain("WITH (UPDLOCK, ROWLOCK)");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Handle_query_with_join(SelectForUpdateCommandInterceptor sut)
    {
        var command = new SqlCommand($"-- {EntityFrameworkInterceptorTags.ForUpdate}{Environment.NewLine}SELECT * FROM [dbo].[Widgets] JOIN [dbo].[Items] ON [Widgets].[Id]=[Items].[WidgetId]");

        sut.ReaderExecuting(command, null!, default);

        command.CommandText.ShouldContain("WITH (UPDLOCK, ROWLOCK)");
    }
}
