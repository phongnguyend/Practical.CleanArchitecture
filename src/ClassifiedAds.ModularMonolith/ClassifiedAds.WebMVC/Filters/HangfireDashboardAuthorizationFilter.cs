using Hangfire.Dashboard;

namespace ClassifiedAds.WebMVC.Filters
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                httpContext.Response.Redirect("/home/login");
            }

            return true;
        }
    }
}
