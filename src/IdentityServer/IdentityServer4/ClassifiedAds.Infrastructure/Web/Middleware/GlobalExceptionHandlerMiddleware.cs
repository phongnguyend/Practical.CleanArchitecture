using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Web.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly GlobalExceptionHandlerMiddlewareOptions _options;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        GlobalExceptionHandlerMiddlewareOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (ex)
            {
                case ValidationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    _logger.LogError(ex, $"[{DateTime.UtcNow.Ticks}-{Environment.CurrentManagedThreadId}]");
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new { message = GetErrorMessage(ex) });
            await response.WriteAsync(result);
        }
    }

    private string GetErrorMessage(Exception ex)
    {
        if (ex is ValidationException)
        {
            return ex.Message;
        }

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

public class GlobalExceptionHandlerMiddlewareOptions
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
