using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Persistence.Interceptors;

public class SelectWhereInCommandInterceptor : DbCommandInterceptor
{
    private static readonly string LOG_TEMPLATE = "SELECT WHERE IN: " + Environment.NewLine + " {Query} " + Environment.NewLine + " {StackTrace}";

    private readonly ILogger _logger;

    public SelectWhereInCommandInterceptor(ILogger logger)
    {
        _logger = logger;
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        CheckCommand(command);

        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        CheckCommand(command);

        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }

    private void CheckCommand(DbCommand command)
    {
        var query = command.CommandText;

        bool selectWhereIn = query.Contains("SELECT", StringComparison.OrdinalIgnoreCase)
            && query.Contains("WHERE", StringComparison.OrdinalIgnoreCase)
            && query.Contains(" IN (", StringComparison.OrdinalIgnoreCase);

        if (selectWhereIn)
        {
            var stackTrace = string.Join("\n", Environment.StackTrace.Split('\n')
                .Where(x => x.Contains("at ClassifiedAds."))
                .Select(x => x));

            _logger.LogWarning(LOG_TEMPLATE, command.CommandText, stackTrace);
        }
    }
}
