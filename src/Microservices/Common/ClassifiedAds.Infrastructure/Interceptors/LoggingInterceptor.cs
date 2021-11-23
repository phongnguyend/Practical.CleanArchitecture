using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassifiedAds.Infrastructure.Interceptors
{
    public class LoggingInterceptor : IInterceptor
    {
        private readonly ILogger _logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
            var className = method.DeclaringType.Name;
            var methodName = method.Name;

            var arguments = JsonSerializer.Serialize(invocation.Arguments, new JsonSerializerOptions()
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            });

            _logger.LogDebug($"Start calling method: {className}.{methodName} with ({arguments}).");

            var watch = new Stopwatch();
            watch.Start();

            invocation.Proceed();

            watch.Stop();

            _logger.LogDebug($"Finished calling method: {className}.{methodName}. Took: {watch.ElapsedMilliseconds} milliseconds");
        }
    }
}
