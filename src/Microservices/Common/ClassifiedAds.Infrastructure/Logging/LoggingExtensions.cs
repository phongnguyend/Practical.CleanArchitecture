using Amazon.Runtime;
using AWS.Logger;
using AWS.Logger.SeriLog;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ClassifiedAds.Infrastructure.Logging;

public static class LoggingExtensions
{
    private static void UseClassifiedAdsLogger(this IWebHostEnvironment env, LoggingOptions options)
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

        var logsPath = Path.Combine(env.ContentRootPath, "logs");
        Directory.CreateDirectory(logsPath);
        var loggerConfiguration = new LoggerConfiguration();

        foreach (var logLevel in options.LogLevel)
        {
            var serilogLevel = ConvertToSerilogLevel(logLevel.Value);

            if (logLevel.Key == "Default")
            {
                loggerConfiguration.MinimumLevel.Is(serilogLevel);
                continue;
            }

            loggerConfiguration.MinimumLevel.Override(logLevel.Key, serilogLevel);
        }

        loggerConfiguration = loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProperty("ProcessId", Environment.ProcessId)
            .Enrich.WithProperty("Assembly", assemblyName)
            .Enrich.WithProperty("Application", env.ApplicationName)
            .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
            .Enrich.WithProperty("ContentRootPath", env.ContentRootPath)
            .Enrich.WithProperty("WebRootPath", env.WebRootPath)
            .Enrich.WithExceptionDetails()
            .Filter.ByIncludingOnly((logEvent) =>
            {
                return true;
            })
            .WriteTo.File(Path.Combine(logsPath, "log.txt"),
                formatProvider: CultureInfo.InvariantCulture,
                fileSizeLimitBytes: 10 * 1024 * 1024,
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] [TraceId: {TraceId}] [MachineName: {MachineName}] [ProcessId: {ProcessId}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: options.File.MinimumLogEventLevel);

        if (options?.AwsCloudWatch?.IsEnabled ?? false)
        {
            loggerConfiguration
                .WriteTo.AWSSeriLog(new AWSLoggerConfig(options.AwsCloudWatch.LogGroup)
                {
                    LogStreamNamePrefix = options.AwsCloudWatch.LogStreamNamePrefix,
                    Region = options.AwsCloudWatch.Region,
                    Credentials = new BasicAWSCredentials(options.AwsCloudWatch.AccessKey, options.AwsCloudWatch.SecretKey),
                },
                iFormatProvider: null,
                textFormatter: new JsonFormatter());
        }

        Log.Logger = loggerConfiguration.CreateLogger();
    }

    private static LoggingOptions SetDefault(LoggingOptions options)
    {
        options ??= new LoggingOptions
        {
        };

        options.LogLevel ??= new Dictionary<string, LogLevel>();

        if (!options.LogLevel.ContainsKey("Default"))
        {
            options.LogLevel["Default"] = LogLevel.Warning;
        }

        options.File ??= new LoggingOptions.FileOptions
        {
            MinimumLogEventLevel = LogEventLevel.Warning,
        };

        options.EventLog ??= new LoggingOptions.EventLogOptions
        {
            IsEnabled = false,
        };

        return options;
    }

    public static IWebHostBuilder UseClassifiedAdsLogger(this IWebHostBuilder builder, Func<IConfiguration, LoggingOptions> logOptions)
    {
        builder.ConfigureLogging((context, logging) =>
        {
            logging.Configure(options =>
            {
            });

            logging.AddAzureWebAppDiagnostics();

            logging.AddSerilog();

            LoggingOptions options = SetDefault(logOptions(context.Configuration));

            if (options.EventLog.IsEnabled && OperatingSystem.IsWindows())
            {
                logging.AddEventLog(configure =>
                {
                    configure.LogName = options.EventLog.LogName;
                    configure.SourceName = options.EventLog.SourceName;
                });
            }

            if (options?.ApplicationInsights?.IsEnabled ?? false)
            {
                logging.AddApplicationInsights(
                    configureTelemetryConfiguration: (config) =>
                    {
                        config.ConnectionString = options.ApplicationInsights.ConnectionString;
                    },
                    configureApplicationInsightsLoggerOptions: (options) => { });
            }

            if (options?.OpenTelemetry?.IsEnabled ?? false)
            {
                var resourceBuilder = ResourceBuilder.CreateDefault().AddService(options.OpenTelemetry.ServiceName);

                logging.AddOpenTelemetry(configure =>
                {
                    configure.SetResourceBuilder(resourceBuilder);

                    if (options?.OpenTelemetry?.Otlp?.IsEnabled ?? false)
                    {
                        configure.AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new Uri(options.OpenTelemetry.Otlp.Endpoint);
                        });
                    }

                    if (options?.OpenTelemetry?.AzureMonitor?.IsEnabled ?? false)
                    {
                        configure.AddAzureMonitorLogExporter(opts =>
                        {
                            opts.ConnectionString = options.OpenTelemetry.AzureMonitor.ConnectionString;
                        });
                    }
                });
            }

            context.HostingEnvironment.UseClassifiedAdsLogger(options);
        });

        return builder;
    }

    public static IHostBuilder UseClassifiedAdsLogger(this IHostBuilder builder, Func<IConfiguration, LoggingOptions> logOptions)
    {
        builder.ConfigureLogging((context, logging) =>
        {
            logging.Configure(options =>
            {
            });

            logging.AddAzureWebAppDiagnostics();

            logging.AddSerilog();

            LoggingOptions options = SetDefault(logOptions(context.Configuration));

            if (OperatingSystem.IsWindows())
            {
                if (options.EventLog.IsEnabled)
                {
                    logging.AddEventLog(configure =>
                    {
                        configure.LogName = options.EventLog.LogName;
                        configure.SourceName = options.EventLog.SourceName;
                    });

                    logging.Services.Configure<LoggerFilterOptions>(options =>
                    {
                        // remove the logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning); added by calling Host.CreateDefaultBuilder(args)
                        // remember to review this in any future update
                        var toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName == "Microsoft.Extensions.Logging.EventLog.EventLogLoggerProvider");

                        if (toRemove is not null)
                        {
                            options.Rules.Remove(toRemove);
                        }
                    });
                }
                else
                {
                    // remove the EventLogLoggerProvider added by calling Host.CreateDefaultBuilder(args) or UseWindowsService()
                    // remember to review this in any future update
                    logging.RemoveEventLog();
                }
            }

            if (options?.ApplicationInsights?.IsEnabled ?? false)
            {
                logging.AddApplicationInsights(
                    configureTelemetryConfiguration: (config) =>
                    {
                        config.ConnectionString = options.ApplicationInsights.ConnectionString;
                    },
                    configureApplicationInsightsLoggerOptions: (options) => { });
            }

            if (options?.OpenTelemetry?.IsEnabled ?? false)
            {
                var resourceBuilder = ResourceBuilder.CreateDefault().AddService(options.OpenTelemetry.ServiceName);

                logging.AddOpenTelemetry(configure =>
                {
                    configure.SetResourceBuilder(resourceBuilder);

                    if (options?.OpenTelemetry?.Otlp?.IsEnabled ?? false)
                    {
                        configure.AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new Uri(options.OpenTelemetry.Otlp.Endpoint);
                        });
                    }

                    if (options?.OpenTelemetry?.AzureMonitor?.IsEnabled ?? false)
                    {
                        configure.AddAzureMonitorLogExporter(opts =>
                        {
                            opts.ConnectionString = options.OpenTelemetry.AzureMonitor.ConnectionString;
                        });
                    }
                });
            }

            context.HostingEnvironment.UseClassifiedAdsLogger(options);
        });

        return builder;
    }

    private static void UseClassifiedAdsLogger(this IHostEnvironment env, LoggingOptions options)
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

        var logsPath = Path.Combine(env.ContentRootPath, "logs");
        Directory.CreateDirectory(logsPath);
        var loggerConfiguration = new LoggerConfiguration();

        foreach (var logLevel in options.LogLevel)
        {
            var serilogLevel = ConvertToSerilogLevel(logLevel.Value);

            if (logLevel.Key == "Default")
            {
                loggerConfiguration.MinimumLevel.Is(serilogLevel);
                continue;
            }

            loggerConfiguration.MinimumLevel.Override(logLevel.Key, serilogLevel);
        }

        loggerConfiguration = loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProperty("ProcessId", Environment.ProcessId)
            .Enrich.WithProperty("Assembly", assemblyName)
            .Enrich.WithProperty("Application", env.ApplicationName)
            .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
            .Enrich.WithProperty("ContentRootPath", env.ContentRootPath)
            .Enrich.WithExceptionDetails()
            .Filter.ByIncludingOnly((logEvent) =>
            {
                return true;
            })
            .WriteTo.File(Path.Combine(logsPath, "log.txt"),
                formatProvider: CultureInfo.InvariantCulture,
                fileSizeLimitBytes: 10 * 1024 * 1024,
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] [TraceId: {TraceId}] [MachineName: {MachineName}] [ProcessId: {ProcessId}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: options.File.MinimumLogEventLevel);

        if (options?.AwsCloudWatch?.IsEnabled ?? false)
        {
            loggerConfiguration
            .WriteTo.AWSSeriLog(new AWSLoggerConfig(options.AwsCloudWatch.LogGroup)
            {
                LogStreamName = options.AwsCloudWatch.LogStreamNamePrefix,
                Region = options.AwsCloudWatch.Region,
                Credentials = new BasicAWSCredentials(options.AwsCloudWatch.AccessKey, options.AwsCloudWatch.SecretKey),
            },
                iFormatProvider: null,
                textFormatter: new JsonFormatter());
        }

        Log.Logger = loggerConfiguration.CreateLogger();
    }

    private static LogEventLevel ConvertToSerilogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            LogLevel.None => LogEventLevel.Fatal,
            _ => LogEventLevel.Fatal
        };
    }

    public static ILoggingBuilder RemoveEventLog(this ILoggingBuilder builder)
    {
        var providers = builder.Services.Where(x => x.ServiceType == typeof(ILoggerProvider)).ToList();

        foreach (var provider in providers)
        {
            if (provider.ImplementationType == typeof(EventLogLoggerProvider))
            {
                builder.Services.Remove(provider);
            }
        }

        return builder;
    }
}
