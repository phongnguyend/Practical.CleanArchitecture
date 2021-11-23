using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassifiedAds.Infrastructure.Interceptors
{
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
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                var method = invocation.Method;
                var className = method.DeclaringType.Name;
                var methodName = method.Name;

                // TODO: Ignore serialize large argument object.
                var arguments = JsonSerializer.Serialize(invocation.Arguments, new JsonSerializerOptions()
                {
                    WriteIndented = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                });
                _logger.LogError($"An unhandled exception has occurred while executing the method: {className}.{methodName} with ({arguments}). {Environment.NewLine}{ex.ToString()}");
                throw;
            }
        }
    }
}
