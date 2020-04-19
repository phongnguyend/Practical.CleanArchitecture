using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace ClassifiedAds.WebMVC.Filters
{
    public class CustomResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Items["CustomResultFilter"] = Stopwatch.StartNew();
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            Stopwatch stopwatch = (Stopwatch)context.HttpContext.Items["CustomResultFilter"];
            var timeElapsed = stopwatch.Elapsed;
        }
    }
}
