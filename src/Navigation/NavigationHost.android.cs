using AndroidX.Fragment.App;
using P41.Navigation.Host;
using System;

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
    protected override object PlatformNavigate(Fragment view)
    {
        var key = Host.BackStackEntryCount.ToString();
        var containerId = FragmentContainerId ??
            throw new NullReferenceException($"{nameof(FragmentContainerId)} was not set! Please set it before using the service.");

        Host
            .BeginTransaction()
            .Replace(containerId, view, key)
            .AddToBackStack(key)
            .Commit();

        Host.ExecutePendingTransactions();

        return view;
    }

    /// <inheritdoc/>
    protected override object? PlatformGoBack()
    {
        Host.PopBackStackImmediate();

        var last = (Host.BackStackEntryCount - 1).ToString();

        return Host.FindFragmentByTag(last);
    }
}
