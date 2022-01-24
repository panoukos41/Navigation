global using Core;
global using ReactiveMarbles.ObservableEvents;
global using ReactiveUI;
global using System;
global using System.Linq;
global using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using P41.Navigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UI.Uwp;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
sealed partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        if (Window.Current.Content is null)
        {
            ConfigureServices();
            var shell = new Shell(Services.Resolve<NavigationHost>());

            Window.Current.Content = shell;
            ApplicationView.GetForCurrentView().Title = "UWP Application";
        }
        Window.Current.Activate();
    }

    private static void ConfigureServices() =>
        Services.Initialize(static services =>
        {
            services.AddSingleton(sp => new NavigationHost()
                .Map("page1/{?}", static () => typeof(Page1))
                .Map("page2", static () => typeof(Page2))
                .Map("page3", static () => typeof(Page3)));

            services.AddSingleton<INavigationHost>(sp => sp.GetRequiredService<NavigationHost>());
        });
}
