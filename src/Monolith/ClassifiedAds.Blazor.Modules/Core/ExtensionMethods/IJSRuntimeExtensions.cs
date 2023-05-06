using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules;

public static class IJSRuntimeExtensions
{
    public static async Task Log(this IJSRuntime jSRuntime, object obj)
    {
        await jSRuntime.InvokeVoidAsync("console.log", $"[{DateTimeOffset.Now}]", obj);
    }

    public static async Task Table(this IJSRuntime jSRuntime, object obj)
    {
        await jSRuntime.InvokeVoidAsync("console.log", $"[{DateTimeOffset.Now}]");
        await jSRuntime.InvokeVoidAsync("console.table", obj);
    }

    public static async Task Alert(this IJSRuntime jSRuntime, string text)
    {
      await jSRuntime.InvokeVoidAsync("alert", text);
    }
}
