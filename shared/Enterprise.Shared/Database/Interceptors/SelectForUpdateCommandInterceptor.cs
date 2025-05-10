using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Enterprise.Shared.Database.Interceptors;

/// <summary>
///     Update SQL commands to include FOR UPDATE commands if tagged
///     https://stackoverflow.com/a/75086260
///     https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/interceptors#example-command-interception-to-add-query-hints
/// </summary>
public class SelectForUpdateCommandInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        ManipulateCommand(command);

        return result;
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        ManipulateCommand(command);

        return new ValueTask<InterceptionResult<object>>(result);
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        ManipulateCommand(command);

        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        ManipulateCommand(command);

        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }

    private static void ManipulateCommand(IDbCommand command)
    {
        if (command.CommandText.StartsWith("-- " + EntityFrameworkInterceptorTags.ForUpdateSkipLocked, StringComparison.Ordinal))
        {
            command.CommandText += " FOR UPDATE SKIP LOCKED";
        }
    }
}
