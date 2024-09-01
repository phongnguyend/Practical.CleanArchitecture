using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.HealthChecks;

public class HttpHealthCheck : IHealthCheck
{
    private static HttpClient _httpClient = new HttpClient();
    private readonly string _uri;

    public HttpHealthCheck(string uri)
    {
        _uri = uri;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(_uri, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy($"Uri: '{_uri}', StatusCode: '{response.StatusCode}'");
            }
            else
            {
                return new HealthCheckResult(context.Registration.FailureStatus, $"Uri: '{_uri}', StatusCode: '{response.StatusCode}'");
            }
        }
        catch (Exception exception)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, $"Uri: '{_uri}', Exception: '{exception.Message}'", exception);
        }
    }
}
