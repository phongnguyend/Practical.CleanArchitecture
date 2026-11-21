using ClassifiedAds.Mobile.Features.Workspace;

namespace ClassifiedAds.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = Enum.Parse<AppTheme>(Preferences.Get("theme", "Unspecified"));
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var page = new WorkspacePage();
        var window = new Window(page);
        window.Stopped += async (_, _) => await page.Pause();
        window.Resumed += (_, _) => page.Resume();
        return window;
    }
}
