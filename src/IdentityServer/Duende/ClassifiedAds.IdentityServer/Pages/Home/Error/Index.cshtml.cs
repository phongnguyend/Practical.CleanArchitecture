using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerHost.Pages.Error;

[AllowAnonymous]
[SecurityHeaders]
public class Index : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IWebHostEnvironment _environment;
        
    public ViewModel View { get; set; }
        
    public Index(IIdentityServerInteractionService interaction, IWebHostEnvironment environment)
    {
        _interaction = interaction;
        _environment = environment;
    }
        
    public async Task OnGet(string errorId)
    {
        View = new ViewModel();

        // retrieve error details from identityserver
        var message = await _interaction.GetErrorContextAsync(errorId);
        if (message != null)
        {
            View.Error = message;

            if (!_environment.IsDevelopment())
            {
                // only show in development
                message.ErrorDescription = null;
            }
        }
    }
}