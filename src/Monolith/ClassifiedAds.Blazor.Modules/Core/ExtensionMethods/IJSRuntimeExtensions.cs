using Microsoft.JSInterop;
using System;

namespace ClassifiedAds.Blazor.Modules
{
    public static class IJSRuntimeExtensions
    {
        public static async void Log(this IJSRuntime jSRuntime, object obj)
        {
            await jSRuntime.InvokeVoidAsync("console.log", $"[{DateTimeOffset.Now}]", obj);
        }

        public static async void Table(this IJSRuntime jSRuntime, object obj)
        {
            await jSRuntime.InvokeVoidAsync("console.log", $"[{DateTimeOffset.Now}]");
            await jSRuntime.InvokeVoidAsync("console.table", obj);
        }

        public static async void Alert(this IJSRuntime jSRuntime, string text)
        {
          await jSRuntime.InvokeVoidAsync("alert", text);
        }
    }
}
