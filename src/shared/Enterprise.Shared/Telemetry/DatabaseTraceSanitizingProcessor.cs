using System.Diagnostics;
using OpenTelemetry;

namespace Enterprise.Shared.Telemetry;

public sealed class DatabaseTraceSanitizingProcessor : BaseProcessor<Activity>
{
    private static readonly string[] s_sensitiveTagNames =
    [
        "db.parameters",
        "db.query.text",
        "db.statement",
        "db.text",
        "db.npgsql.data_source",
        "db.connection_string",
    ];

    public override void OnEnd(Activity data)
    {
        foreach (var tagName in s_sensitiveTagNames)
        {
            data.SetTag(tagName, null);
        }
    }
}
