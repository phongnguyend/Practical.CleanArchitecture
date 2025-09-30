using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Interceptors;

public class ErrorCatchingInterceptor : IInterceptor
{
    private readonly ILogger _logger;

    public ErrorCatchingInterceptor(ILogger<ErrorCatchingInterceptor> logger)
    {
        _logger = logger;
    }

    public void Intercept(IInvocation invocation)
    {
        try
        {
            var returnType = invocation.Method.ReturnType;
            if (returnType == typeof(Task))
            {
                invocation.Proceed();
                invocation.ReturnValue = InterceptResultAsync((dynamic)invocation.ReturnValue, invocation);
            }
            else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                invocation.Proceed();
                invocation.ReturnValue = InterceptResultAsync((dynamic)invocation.ReturnValue, invocation);
            }
            else
            {
                invocation.Proceed();
            }
        }
        catch (Exception ex)
        {
            LogException(invocation, ex);
            throw;
        }
    }

    private async Task InterceptResultAsync(Task task, IInvocation invocation)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            LogException(invocation, ex);
            throw;
        }
    }

    private async Task<T> InterceptResultAsync<T>(Task<T> task, IInvocation invocation)
    {
        try
        {
            T result = await task.ConfigureAwait(false);
            return result;
        }
        catch (Exception ex)
        {
            LogException(invocation, ex);
            throw;
        }
    }

    private void LogException(IInvocation invocation, Exception ex)
    {
        var method = invocation.Method;
        var className = method.DeclaringType.Name;
        var methodName = method.Name;

        // TODO: Ignore serialize large argument object.
        var arguments = JsonSerializer.Serialize(invocation.Arguments.Where(x => x.GetType() != typeof(CancellationToken)), new JsonSerializerOptions()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        });

        _logger.LogError($"An unhandled exception has occurred while executing the method: {className}.{methodName} with ({arguments}). {Environment.NewLine}{ex}");
    }
}
