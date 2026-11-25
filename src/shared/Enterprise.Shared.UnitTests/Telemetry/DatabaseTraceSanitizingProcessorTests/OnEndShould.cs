using System.Diagnostics;
using Enterprise.Shared.Telemetry;

namespace Enterprise.Shared.UnitTests.Telemetry.DatabaseTraceSanitizingProcessorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OnEndShould
{
    [Fact]
    public void Remove_Database_Credentials_Queries_And_Parameters()
    {
        using var activity = new Activity("database-command");
        activity.SetTag("db.statement", "select secret from customer");
        activity.SetTag("db.parameters", "customerId=123");
        activity.SetTag("db.npgsql.data_source", "Host=postgres;Username=user;Password=secret");
        activity.SetTag("db.system", "postgresql");

        new DatabaseTraceSanitizingProcessor().OnEnd(activity);

        activity.GetTagItem("db.statement").ShouldBeNull();
        activity.GetTagItem("db.parameters").ShouldBeNull();
        activity.GetTagItem("db.npgsql.data_source").ShouldBeNull();
        activity.GetTagItem("db.system").ShouldBe("postgresql");
    }
}
