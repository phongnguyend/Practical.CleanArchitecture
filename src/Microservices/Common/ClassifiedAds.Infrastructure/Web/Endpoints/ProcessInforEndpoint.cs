using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Diagnostics;
using System.Globalization;

namespace ClassifiedAds.Infrastructure.Web.Endpoints;

public static class ProcessInforEndpoint
{
    public static void MapProcessInforEndpoint(this IEndpointRouteBuilder builder, string path = "/processinfor")
    {
        builder.MapGet(path, () =>
        {
            var currentProcess = Process.GetCurrentProcess();

            return Results.Ok(new
            {
                Timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture),
                MachineName = Environment.MachineName,
                ProcessId = Environment.ProcessId,
                UserName = Environment.UserName,
                TotalProcessorTime = currentProcess.TotalProcessorTime.TotalSeconds,
                PrivateMemorySize64 = currentProcess.PrivateMemorySize64 / 1024 / 1024,
                VirtualMemorySize64 = currentProcess.VirtualMemorySize64 / 1024 / 1024,
                WorkingSet64 = currentProcess.WorkingSet64 / 1024 / 1024,
                PeakVirtualMemorySize64 = currentProcess.PeakVirtualMemorySize64 / 1024 / 1024,
                PeakWorkingSet64 = currentProcess.PeakWorkingSet64 / 1024 / 1024
            });
        });
    }
}
