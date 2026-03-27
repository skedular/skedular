using System.Data;
using System.Data.Common;
using Enterprise.Shared.Database.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Enterprise.Shared.Database.SqlServer.Interceptors;

/// <summary>
///     Update SQL commands to include SQL Server lock hints if tagged.
///     https://learn.microsoft.com/en-us/sql/t-sql/queries/hints-transact-sql-table
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
            command.CommandText = ReplaceFromWithLockHint(command.CommandText, "UPDLOCK, READPAST, ROWLOCK");
            return;
        }

        if (command.CommandText.StartsWith("-- " + EntityFrameworkInterceptorTags.ForUpdate, StringComparison.Ordinal))
        {
            command.CommandText = ReplaceFromWithLockHint(command.CommandText, "UPDLOCK, ROWLOCK");
        }
    }

    private static string ReplaceFromWithLockHint(string commandText, string lockHints)
    {
        var fromIndex = commandText.IndexOf("FROM ", StringComparison.OrdinalIgnoreCase);
        if (fromIndex == -1)
        {
            return commandText;
        }

        var afterFrom = fromIndex + 5;
        var nextSpace = commandText.IndexOf(' ', afterFrom);
        var nextComma = commandText.IndexOf(',', afterFrom);
        var nextJoin = commandText.IndexOf("JOIN", afterFrom, StringComparison.OrdinalIgnoreCase);

        var endIndex = int.MaxValue;
        if (nextSpace > 0)
        {
            endIndex = Math.Min(endIndex, nextSpace);
        }

        if (nextComma > 0)
        {
            endIndex = Math.Min(endIndex, nextComma);
        }

        if (nextJoin > 0)
        {
            endIndex = Math.Min(endIndex, nextJoin);
        }

        if (endIndex == int.MaxValue)
        {
            endIndex = commandText.Length;
            var semiIndex = commandText.LastIndexOf(';');
            if (semiIndex > afterFrom)
            {
                endIndex = semiIndex;
            }
        }

        var tableIdentifier = commandText[afterFrom..endIndex];
        var hints = $"{tableIdentifier} WITH ({lockHints})";

        return commandText[..afterFrom] + hints + commandText[endIndex..];
    }
}
