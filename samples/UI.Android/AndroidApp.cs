global using Android.App;
global using Android.Views;
global using Core;
global using ReactiveUI.AndroidX;
global using R = UI.Android.Resource;
using Android.Runtime;
using Microsoft.Extensions.DependencyInjection;
using P41.Navigation;
using UI.Android.Pages;

namespace Droid;

[Application]
public class AndroidApp : Application
{
    public AndroidApp(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        Services.Initialize(services =>
        {
            services.AddSingleton(sp =>
                new NavigationHost()
                .Map("page1", static () => new Page1())
                .Map("page2", static () => new Page2())
                .Map("page3", static () => new Page3())
            );

            services.AddSingleton<INavigationHost>(sp => sp.GetRequiredService<NavigationHost>());
        });
    }
}
