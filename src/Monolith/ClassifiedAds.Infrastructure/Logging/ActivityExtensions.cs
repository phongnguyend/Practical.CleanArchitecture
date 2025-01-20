using System.Diagnostics;

namespace ClassifiedAds.Infrastructure.Logging;

public static class ActivityExtensions
{
    public static string GetSpanId(this Activity activity)
    {
        return activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.Id,
            ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
            _ => null,
        } ?? string.Empty;
    }

    public static string GetTraceId(this Activity activity)
    {
        return activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.RootId,
            ActivityIdFormat.W3C => activity.TraceId.ToHexString(),
            _ => null,
        } ?? string.Empty;
    }

    public static string GetParentId(this Activity activity)
    {
        return activity.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.ParentId,
            ActivityIdFormat.W3C => activity.ParentSpanId.ToHexString(),
            _ => null,
        } ?? string.Empty;
    }

    public static Activity StartNew(string name)
    {
        var activity = new Activity(name);
        activity.Start();
        return activity;
    }

    public static Activity StartNew(string name, string traceId)
    {
        var activity = new Activity(name);
        activity.SetIdFormat(ActivityIdFormat.W3C);
        activity.SetTag("TraceId", traceId);
        activity.Start();
        return activity;
    }
}
