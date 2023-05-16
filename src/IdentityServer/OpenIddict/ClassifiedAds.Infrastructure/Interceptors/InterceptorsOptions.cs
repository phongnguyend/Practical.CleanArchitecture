using System;
using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.Interceptors;

public class InterceptorsOptions
{
    public bool LoggingInterceptor { get; set; }

    public bool ErrorCatchingInterceptor { get; set; }

    public Type[] GetInterceptors()
    {
        var interceptors = new List<Type>();
        if (LoggingInterceptor)
        {
            interceptors.Add(typeof(LoggingInterceptor));
        }

        if (ErrorCatchingInterceptor)
        {
            interceptors.Add(typeof(ErrorCatchingInterceptor));
        }

        return interceptors.ToArray();
    }
}
