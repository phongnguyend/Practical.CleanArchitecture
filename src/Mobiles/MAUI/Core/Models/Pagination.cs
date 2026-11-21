namespace ClassifiedAds.Mobile.Core.Models;

public sealed class Pagination(int pageSize = 5)
{
    public int Page { get; private set; } = 1;

    public int Total { get; private set; }

    public bool HasPrevious => Page > 1;
    public bool HasNext => Page * pageSize < Total;
    public string Summary => $"Page {Page} · {Total} records";

    public void Reset() => Page = 1;
    public void SetTotal(int total)
    {
        Total = Math.Max(0, total);
        Page = Math.Min(Page, Math.Max(1, (Total + pageSize - 1) / pageSize));
    }

    public void Next()
    {
        if (HasNext)
        {
            Page++;
        }
    }

    public void Previous()
    {
        if (HasPrevious)
        {
            Page--;
        }
    }

    public IEnumerable<T> Slice<T>(IEnumerable<T> values) => values.Skip((Page - 1) * pageSize).Take(pageSize);
}
