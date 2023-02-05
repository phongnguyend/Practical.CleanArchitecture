using ClassifiedAds.CrossCuttingConcerns.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;

namespace ClassifiedAds.Infrastructure.Web.Filters
{
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
                context.Result = new BadRequestObjectResult(context.Exception.Message);
            }
            else
            {
                _logger.LogError(context.Exception, $"[{DateTime.UtcNow.Ticks}-{Environment.CurrentManagedThreadId}]");

                if (_options.DetailLevel == GlobalExceptionDetailLevel.Throw)
                {
                    return;
                }

                context.Result = new ObjectResult(new { Message = GetErrorMessage(context.Exception) })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
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
}
