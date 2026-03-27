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
}
