using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using ClassifiedAds.Infrastructure.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net;

namespace ClassifiedAds.Infrastructure.Web.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly GlobalExceptionFilterOptions _options;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger,
        IOptionsSnapshot<GlobalExceptionFilterOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is NotFoundException)
        {
            context.Result = new NotFoundResult();
        }
        else if (context.Exception is ValidationException)
        {
            var problemDetails = new ProblemDetails
            {
                Detail = context.Exception.Message,
                Instance = null,
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Bad Request",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
            };

            problemDetails.Extensions.Add("message", context.Exception.Message);
            problemDetails.Extensions.Add("traceId", Activity.Current.GetTraceId());

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status,
            };
        }
        else
        {
            _logger.LogError(context.Exception, "[{Ticks}-{ThreadId}]", DateTime.UtcNow.Ticks, Environment.CurrentManagedThreadId);

            if (_options.DetailLevel == GlobalExceptionDetailLevel.Throw)
            {
                return;
            }

            var problemDetails = new ProblemDetails
            {
                Detail = GetErrorMessage(context.Exception),
                Instance = null,
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Internal Server Error",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            };

            problemDetails.Extensions.Add("message", GetErrorMessage(context.Exception));
            problemDetails.Extensions.Add("traceId", Activity.Current.GetTraceId());

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status,
            };
        }
    }

    private string GetErrorMessage(Exception ex)
    {
        return _options.DetailLevel switch
        {
            GlobalExceptionDetailLevel.None => "An internal exception has occurred.",
            GlobalExceptionDetailLevel.Message => ex.Message,
            GlobalExceptionDetailLevel.StackTrace => ex.StackTrace,
            GlobalExceptionDetailLevel.ToString => ex.ToString(),
            _ => "An internal exception has occurred.",
        };
    }
}

public class GlobalExceptionFilterOptions
{
    public GlobalExceptionDetailLevel DetailLevel { get; set; }
}

public enum GlobalExceptionDetailLevel
{
    None,
    Message,
    StackTrace,
    ToString,
    Throw,
}
