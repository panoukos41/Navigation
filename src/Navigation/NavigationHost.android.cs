using AndroidX.Fragment.App;
using P41.Navigation.Host;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace P41.Navigation;

public class NavigationHost : NavigationHostBase<FragmentManager, Fragment, NavigationHost>
{
    /// <summary>
    /// Gets or sets the FragmentContainerId that should be used for the navigation.
    /// </summary>
    public virtual int? FragmentContainerId { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="NavigationHost"/>.
    /// </summary>
    public NavigationHost()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NavigationHost"/> with
    /// values for the containerId and the fragmentmanager.
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="fragmentContainerId"></param>
    public NavigationHost(FragmentManager manager, int fragmentContainerId)
    {
        Host = manager;
        FragmentContainerId = fragmentContainerId;
    }

    /// <summary>
    /// </summary>
    /// <param name="fragmentManager"></param>
    /// <returns></returns>
    public virtual NavigationHost SetFragmentManager(FragmentManager fragmentManager)
    {
        Host = fragmentManager;
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="fragmentContainerId"></param>
    /// <returns></returns>
    public virtual NavigationHost SetFragmentContainerId(int fragmentContainerId)
    {
        FragmentContainerId = fragmentContainerId;
        return this;
    }

    /// <inheritdoc/>
    protected override IObservable<IViewFor> PlatformNavigate()
    {
        var manager = Host;

        var fragment = InitializeView();

        if (fragment is IViewFor view)
        {
            // We initialize the ViewModel now since most of the times
            // the platform is much faster at creating views than us
            // injecting the ViewModel
            view.ViewModel = InitializeViewModel();
        }

        var key = manager.BackStackEntryCount.ToString();
        var containerId = FragmentContainerId ??
            throw new NullReferenceException($"{nameof(FragmentContainerId)} was not set! Please set it before using the service.");

        manager
            .BeginTransaction()
            .Replace(containerId, fragment, key)
            .AddToBackStack(key)
            .Commit();

        manager.ExecutePendingTransactions();

        return GetHostContent(manager);
    }

    /// <inheritdoc/>
    protected override IObservable<IViewFor?> PlatformGoBack()
    {
        var manager = Host;

        manager.PopBackStackImmediate();

        return GetHostContent(manager);
    }

    private static IObservable<IViewFor> GetHostContent(FragmentManager manager)
    {
        var last = (manager.BackStackEntryCount - 1).ToString();

        return manager.FindFragmentByTag(last) is IViewFor view
            ? Observable.Return(view)
            : throw new ArgumentException($"View must implement {nameof(IViewFor)}");
    }
}
