using System.Diagnostics;

namespace ClassifiedAds.CrossCuttingConcerns.Logging;

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

    public static Activity StartNew(string name, string parentId = null)
    {
        var activity = new Activity(name);

        if (!string.IsNullOrEmpty(parentId))
        {
            activity.SetParentId(parentId);
        }

        activity.Start();
        return activity;
    }
}
