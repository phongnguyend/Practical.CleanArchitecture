using System;

namespace ClassifiedAds.Infrastructure.Web.ExceptionHandlers;

public class GlobalExceptionHandlerOptions
{
    public GlobalExceptionDetailLevel DetailLevel { get; set; }

    public string GetErrorMessage(Exception ex)
    {
        return DetailLevel switch
        {
            GlobalExceptionDetailLevel.None => "An internal exception has occurred.",
            GlobalExceptionDetailLevel.Message => ex.Message,
            GlobalExceptionDetailLevel.StackTrace => ex.StackTrace,
            GlobalExceptionDetailLevel.ToString => ex.ToString(),
            _ => "An internal exception has occurred.",
        };
    }
}

public enum GlobalExceptionDetailLevel
{
    None,
    Message,
    StackTrace,
    ToString,
    Throw,
}