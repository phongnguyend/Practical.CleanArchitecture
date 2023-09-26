using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.HostedServices;

/// <summary>
/// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-7.0#ihostapplicationlifetime
/// </summary>
public class HostApplicationLifetimeEventsHostedService : IHostedService
{
    private readonly ILogger<HostApplicationLifetimeEventsHostedService> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private const string _logFolder = "logs";
    private readonly string _logFile = Path.Combine(_logFolder, "HostApplicationLifetimeEvents.txt");

    public HostApplicationLifetimeEventsHostedService(
        ILogger<HostApplicationLifetimeEventsHostedService> logger,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_logFolder))
        {
            Directory.CreateDirectory(_logFolder);
        }

        _hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
        _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
        _hostApplicationLifetime.ApplicationStopped.Register(OnStopped);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void OnStarted()
    {
        try
        {
            var message = $"{GetMessagePrefix()}: Application Started.";

            File.AppendAllLines(_logFile, new[] { message });
        }
        catch (Exception)
        {
            _hostApplicationLifetime.StopApplication();
            throw;
        }
    }

    private void OnStopping()
    {
        var message = $"{GetMessagePrefix()}: Application Stopping.";
        File.AppendAllLines(_logFile, new[] { message });
    }

    private void OnStopped()
    {
        var message = $"{GetMessagePrefix()}: Application Stopped.";
        File.AppendAllLines(_logFile, new[] { message });
    }

    private static string GetMessagePrefix()
    {
        var currentProcess = Process.GetCurrentProcess();

        var message = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} [MachineName: {Environment.MachineName}] [ProcessId: {Environment.ProcessId}]";

        message += $" [UserName: {Environment.UserName}]";
        message += $" [TotalProcessorTime: {currentProcess.TotalProcessorTime.TotalSeconds} Seconds]";
        message += $" [PrivateMemorySize64: {currentProcess.PrivateMemorySize64 / 1024 / 1024} MB]";
        message += $" [VirtualMemorySize64: {currentProcess.VirtualMemorySize64 / 1024 / 1024} MB]";
        message += $" [WorkingSet64: {currentProcess.WorkingSet64 / 1024 / 1024} MB]";
        message += $" [PeakVirtualMemorySize64: {currentProcess.PeakVirtualMemorySize64 / 1024 / 1024} MB]";
        message += $" [PeakWorkingSet64: {currentProcess.PeakWorkingSet64 / 1024 / 1024} MB]";

        return message;
    }
}
