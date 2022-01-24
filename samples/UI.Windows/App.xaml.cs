global using Core;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using ReactiveMarbles.ObservableEvents;
global using ReactiveUI;
global using System;
global using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using P41.Navigation;

namespace UI.Windows;

public partial class App : Application
{
    private Shell? shell;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        if (shell is null)
        {
            ConfigureServices();
            shell = new Shell(Services.Resolve<NavigationHost>());
            shell.Title = "Win UI 3 Application";
        }
        shell.Activate();
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
