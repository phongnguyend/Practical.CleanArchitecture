using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Globalization;

namespace ClassifiedAds.Infrastructure.Web.Endpoints;

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/api/system.gc
/// </summary>
public static class GcInforEndpoint
{
    public static void MapGcInforEndpoint(this IEndpointRouteBuilder builder, string path = "/gcinfor")
    {
        builder.MapGet(path, () =>
        {
            var gcMemoryInfo = GC.GetGCMemoryInfo();

            return Results.Ok(new
            {
                Timestamp = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz", CultureInfo.InvariantCulture),
                MachineName = Environment.MachineName,
                ProcessId = Environment.ProcessId,
                TotalMemory = GC.GetTotalMemory(false),
                Gen0Collections = GC.CollectionCount(0),
                Gen1Collections = GC.CollectionCount(1),
                Gen2Collections = GC.CollectionCount(2),
                TotalPauseDuration = GC.GetTotalPauseDuration(),
                GCMemoryInfo = new
                {
                    gcMemoryInfo.HighMemoryLoadThresholdBytes,
                    gcMemoryInfo.MemoryLoadBytes,
                    gcMemoryInfo.HeapSizeBytes,
                    gcMemoryInfo.FragmentedBytes,
                    gcMemoryInfo.PauseTimePercentage,
                    gcMemoryInfo.PinnedObjectsCount
                }
            });
        });
    }
}
