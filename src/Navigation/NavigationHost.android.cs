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
    protected override IObservable<object> PlatformNavigate()
    {
        var manager = Host;

        var fragment = InitializeView();

        var key = manager.BackStackEntryCount.ToString();
        var containerId = FragmentContainerId ??
            throw new NullReferenceException($"{nameof(FragmentContainerId)} was not set! Please set it before using the service.");

        SetViewModel(fragment as IViewFor);

        manager
            .BeginTransaction()
            .Replace(containerId, fragment, key)
            .AddToBackStack(key)
            .Commit();

        manager.ExecutePendingTransactions();

        return GetHostContent(manager);
    }

    /// <inheritdoc/>
    protected override IObservable<object?> PlatformGoBack()
    {
        var manager = Host;

        manager.PopBackStackImmediate();

        return GetHostContent(manager);
    }

    private static IObservable<object> GetHostContent(FragmentManager manager)
    {
        var last = (manager.BackStackEntryCount - 1).ToString();

        return Observable.Return(manager.FindFragmentByTag(last));
    }
}
