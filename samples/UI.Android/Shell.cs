using Google.Android.Material.Button;
using P41.Navigation;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace UI.Android;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
public class Shell : ReactiveAppCompatActivity<ShellViewModel>
{
    private CompositeDisposable disposables = null!;

    public MaterialButton Page1Button { get; private set; } = null!;
    public MaterialButton Page2Button { get; private set; } = null!;
    public MaterialButton Page3Button { get; private set; } = null!;

    private NavigationHost host = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(R.Layout.activity_main);

        host = Services.Resolve<NavigationHost>();
        host.ShouldPopRoot = static () => true;

        this.WireUpControls();
        this.ResolveViewModel();

        this.BindCommand(ViewModel, vm => vm.Navigate, v => v.Page1Button, Observable.Return("page1"));
        this.BindCommand(ViewModel, vm => vm.Navigate, v => v.Page2Button, Observable.Return("page2"));
        this.BindCommand(ViewModel, vm => vm.Navigate, v => v.Page3Button, Observable.Return("page3"));

        disposables = new()
        {
            host.WhenRootPopped.Subscribe(_ => Finish())
        };

        host.SetFragmentManager(SupportFragmentManager)
            .SetFragmentContainerId(R.Id.MainContainer)
            .Navigate("page1");
    }

    public override void OnBackPressed()
    {
        host.GoBack();
    }

    protected override void OnDestroy()
    {
        disposables.Dispose();
        base.OnDestroy();
    }
}
