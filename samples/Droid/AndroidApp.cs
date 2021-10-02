using Android.Runtime;
using Microsoft.Extensions.DependencyInjection;
using P41.Navigation;
using System;

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
                .Map("page1", static () => Services.Resolve<Page1ViewModel>(), static () => new Page1())
                .Map("page2", static () => Services.Resolve<Page2ViewModel>(), static () => new Page2())
                .Map("page3", static () => Services.Resolve<Page3ViewModel>(), static () => new Page3())
            );

            services.AddSingleton<INavigationHost>(sp => sp.GetRequiredService<NavigationHost>());
        });
    }
}
