using ClassifiedAds.CrossCuttingConcerns.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.HostedServices;

public class HealthChecksBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<HealthChecksBackgroundService> _logger;
    private readonly HealthCheckService _healthCheckService;
    private readonly HealthChecksBackgroundServiceOptions _options;

    public HealthChecksBackgroundService(IServiceProvider services,
        ILogger<HealthChecksBackgroundService> logger,
        HealthCheckService healthCheckService,
        IOptions<HealthChecksBackgroundServiceOptions> options)
    {
        _services = services;
        _logger = logger;
        _healthCheckService = healthCheckService;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var jsonOptions = new JsonWriterOptions { Indented = true };

        while (!stoppingToken.IsCancellationRequested)
        {
            using var activity = ActivityExtensions.StartNew("HealthChecksBackgroundService");

            var healthReport = await _healthCheckService.CheckHealthAsync(stoppingToken);

            using var memoryStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(memoryStream, jsonOptions))
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WriteString("status", healthReport.Status.ToString());
                jsonWriter.WriteStartObject("results");

                foreach (var healthReportEntry in healthReport.Entries)
                {
                    jsonWriter.WriteStartObject(healthReportEntry.Key);
                    jsonWriter.WriteString("status", healthReportEntry.Value.Status.ToString());
                    jsonWriter.WriteString("description", healthReportEntry.Value.Description ?? healthReportEntry.Value.Exception?.Message.ToString());
                    jsonWriter.WriteStartObject("data");

                    foreach (var item in healthReportEntry.Value.Data)
                    {
                        jsonWriter.WritePropertyName(item.Key);

                        JsonSerializer.Serialize(jsonWriter, item.Value, item.Value?.GetType() ?? typeof(object));
                    }

                    jsonWriter.WriteEndObject();

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            var json = Encoding.UTF8.GetString(memoryStream.ToArray());

            if (healthReport.Status == HealthStatus.Healthy)
            {
                _logger.LogInformation(json);
            }
            else if (healthReport.Status == HealthStatus.Degraded)
            {
                _logger.LogWarning(json);
            }
            else
            {
                _logger.LogError(json);
            }

            await Task.Delay(_options.Interval, stoppingToken);
        }
    }
}

public class HealthChecksBackgroundServiceOptions
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(10);
}
