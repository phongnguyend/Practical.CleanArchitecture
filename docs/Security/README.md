## Browser Security Headers

### Use handcrafted class:

SecurityHeadersMiddleware.cs
```cs
public static class IApplicationBuilderExtensions
{
    public static void UseSecurityHeaders(this IApplicationBuilder app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        //// recommended security headers, adjust/add/remove them based on your requirements

        context.Response.Headers.Add("Content-Security-Policy", "form-action 'self'; frame-ancestors 'none'");
        context.Response.Headers.Add("Feature-Policy", "camera 'none'");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

        context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("Expires", "0");

        await _next(context);
    }
}
```

On Startup.cs class, add app.UseSecurityHeaders();

```cs
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseSecurityHeaders();

    ....

    app.UseMvc(routes =>
    {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });
}
```

### Use Community Libraries:
- [NWebsec](https://github.com/NWebsec/NWebsec)
- [aspnetcore-security-headers](https://github.com/juunas11/aspnetcore-security-headers)
- [NetEscapades.AspNetCore.SecurityHeaders](https://github.com/andrewlock/NetEscapades.AspNetCore.SecurityHeaders)
