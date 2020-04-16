using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;
using System;
using System.IO;
using System.Reflection;

namespace ClassifiedAds.Infrastructure.Logging
{
    public static class Logger
    {
        public static void UseLogger(this IWebHostEnvironment env, LoggerOptions options = null)
        {
            options ??= new LoggerOptions
            {
                File = new FileOptions
                {
                    MinimumLogEventLevel = Serilog.Events.LogEventLevel.Debug,
                },
                Elasticsearch = new ElasticsearchOptions
                {
                    IsEnabled = false,
                    MinimumLogEventLevel = Serilog.Events.LogEventLevel.Debug,
                },
            };

            var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

            var logsPath = Path.Combine(env.ContentRootPath, "logs");
            Directory.CreateDirectory(logsPath);
            var loggerConfiguration = new LoggerConfiguration();

            loggerConfiguration = loggerConfiguration
                .MinimumLevel.Debug()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithProperty("Assembly", assemblyName)
                .Enrich.WithProperty("Application", env.ApplicationName)
                .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                .Enrich.WithProperty("ContentRootPath", env.ContentRootPath)
                .Enrich.WithProperty("WebRootPath", env.WebRootPath)
                .Filter.ByIncludingOnly((logEvent) =>
                {
                    if (logEvent.Level >= options.File.MinimumLogEventLevel
                    || logEvent.Level >= options.Elasticsearch.MinimumLogEventLevel)
                    {
                        if (logEvent.Level >= Serilog.Events.LogEventLevel.Warning)
                        {
                            return true;
                        }

                        var sourceContext = logEvent.Properties.ContainsKey("SourceContext")
                             ? logEvent.Properties["SourceContext"].ToString()
                             : null;

                        if (sourceContext.Contains("ClassifiedAds."))
                        {
                            return true;
                        }
                    }

                    return false;
                })
                .WriteTo.File(Path.Combine(logsPath, "log.txt"),
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    restrictedToMinimumLevel: options.File.MinimumLogEventLevel);

            if (options.Elasticsearch != null && options.Elasticsearch.IsEnabled)
            {
                loggerConfiguration = loggerConfiguration
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(options.Elasticsearch.Host))
                    {
                        MinimumLogEventLevel = options.Elasticsearch.MinimumLogEventLevel,
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                        IndexFormat = options.Elasticsearch.IndexFormat + "-{0:yyyy.MM.dd}",
                        // BufferBaseFilename = Path.Combine(env.ContentRootPath, "logs", "buffer"),
                        InlineFields = true,
                        EmitEventFailure = EmitEventFailureHandling.WriteToFailureSink,
                        FailureSink = new FileSink(Path.Combine(logsPath, "elasticsearch-failures.txt"), new JsonFormatter(), null)
                    });
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        public static IWebHostBuilder UseLogger(this IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging =>
            {
                //logging.AddEventLog(new EventLogSettings
                //{
                //    LogName = "ClassifiedAds",
                //    SourceName = "WebAPI",
                //    Filter = (a, b) => b >= LogLevel.Information
                //});
            });

            builder.UseSerilog();

            return builder;
        }
    }
}
