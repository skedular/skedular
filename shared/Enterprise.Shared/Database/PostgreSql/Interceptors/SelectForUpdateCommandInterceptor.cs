using System.Data;
using System.Data.Common;
using Enterprise.Shared.Database.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Enterprise.Shared.Database.PostgreSql.Interceptors;

/// <summary>
///     Update SQL commands to include FOR UPDATE commands if tagged
///     https://stackoverflow.com/a/75086260
///     https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/interceptors#example-command-interception-to-add-query-hints
/// </summary>
public class SelectForUpdateCommandInterceptor : DbCommandInterceptor
{
    private readonly ILogger<SelectForUpdateCommandInterceptor> _logger;

    public SelectForUpdateCommandInterceptor(ILogger<SelectForUpdateCommandInterceptor>? logger = null) =>
        _logger = logger ?? NullLogger<SelectForUpdateCommandInterceptor>.Instance;

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

    private void ManipulateCommand(IDbCommand command)
    {
        if (command.CommandText.StartsWith("-- " + EntityFrameworkInterceptorTags.ForUpdate, StringComparison.Ordinal))
        {
            _logger.LogDebug("Applying FOR UPDATE lock hint to PostgreSQL command");
            command.CommandText += " FOR UPDATE";
        }

        if (command.CommandText.StartsWith("-- " + EntityFrameworkInterceptorTags.ForUpdateSkipLocked, StringComparison.Ordinal))
        {
            _logger.LogDebug("Applying FOR UPDATE SKIP LOCKED lock hint to PostgreSQL command");
            command.CommandText += " FOR UPDATE SKIP LOCKED";
        }
    }
}
