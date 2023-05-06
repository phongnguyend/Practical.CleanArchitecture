using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Core.Components;

public partial class Pagination
{
    [Parameter]
    public long TotalItems { get; set; }

    [Parameter]
    public int CurrentPage { get; set; }

    [Parameter]
    public long PageSize { get; set; }

    public int TotalPages
    {
        get
        {
            return PageSize != 0 ? (int)((TotalItems + PageSize - 1) / PageSize) : 0;
        }
    }

    public List<int> PageNumbers { get; private set; }

    [Parameter]
    public EventCallback<int> PageChanged { get; set; }

    protected async Task HandleClick(int page)
    {
        await PageChanged.InvokeAsync(page);
    }

    protected override void OnParametersSet()
    {
        var startIndex = CurrentPage - 2;
        var endIndex = CurrentPage + 2;
        var totalPages = TotalPages;

        if (startIndex < 1)
        {
            endIndex = endIndex + (1 - startIndex);
            startIndex = 1;
        }

        if (endIndex > totalPages)
        {
            startIndex = startIndex - (endIndex - totalPages);
            endIndex = totalPages;
        }

        startIndex = Math.Max(startIndex, 1);
        endIndex = Math.Min(endIndex, totalPages);

        PageNumbers = new List<int>();
        for (var i = startIndex; i <= endIndex; i++)
        {
            PageNumbers.Add(i);
        }
    }
}
