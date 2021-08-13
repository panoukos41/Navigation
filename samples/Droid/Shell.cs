global using Android.App;
global using Android.OS;
global using Android.Views;
global using Base;
global using ReactiveUI.AndroidX;
global using R = Droid.Resource;
using Google.Android.Material.Button;
using Microsoft.Extensions.DependencyInjection;
using P41.Navigation;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Droid;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
public class Shell : ReactiveAppCompatActivity<ShellViewModel>
{
    public MaterialButton Page1Button { get; private set; } = null!;
    public MaterialButton Page2Button { get; private set; } = null!;
    public MaterialButton Page3Button { get; private set; } = null!;

    private INavigationHost host = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        Services.Initialize(services =>
        {
            services.AddSingleton(sp =>
                new NavigationHost(SupportFragmentManager, R.Id.MainContainer)
                .AddPair("page1", static () => new Page1(), static () => Services.Resolve<Page1ViewModel>())
                .AddPair("page2", static () => new Page2(), static () => Services.Resolve<Page2ViewModel>())
                .AddPair("page3", static () => new Page3(), static () => Services.Resolve<Page3ViewModel>())
            );

            services.AddSingleton<INavigationHost>(sp => sp.GetRequiredService<NavigationHost>());
        });
        host = Services.Resolve<NavigationHost>();

        SetContentView(R.Layout.activity_main);

        this.WireUpControls();
        this.ResolveViewModel();

        this.BindCommand(ViewModel, vm => vm.Navigate, v => v.Page1Button, Observable.Return("page1"));
        this.BindCommand(ViewModel, vm => vm.Navigate, v => v.Page2Button, Observable.Return("page2"));
        this.BindCommand(ViewModel, vm => vm.Navigate, v => v.Page3Button, Observable.Return("page3"));

        host.Navigate("page1");
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
    {
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    public override void OnBackPressed()
    {
        host.Pop()
            .Where(static x => x is { IsRoot: true })
            .Subscribe(_ => Finish());
    }
}
