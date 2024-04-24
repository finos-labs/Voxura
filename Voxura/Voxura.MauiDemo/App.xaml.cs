using System.Diagnostics;

namespace Voxura.MauiDemo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        base.OnStart();
        var config = IPlatformApplication.Current.Services.GetRequiredService<ApplicationConfig>();
        if (!config.Verify())
        {
            Shell.Current.GoToAsync(nameof(SettingsPage));
        }
    }
}
