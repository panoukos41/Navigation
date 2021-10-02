using Microsoft.UI.Xaml.Controls;
using P41.Navigation.Host;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace P41.Navigation;

/// <summary>
/// Implementation of <see cref="INavigationHost"/> that can be configured with factory
/// methods for Views and ViewModels.
/// </summary>
public class NavigationHost : NavigationHostBase<Frame, Type, NavigationHost>
{
    /// <summary>
    /// Initialize a new <see cref="NavigationHost"/>.
    /// </summary>
    /// <remarks>Host will be null.</remarks>
    public NavigationHost()
    {
    }

    /// <summary>
    /// Initialize a new <see cref="NavigationHost"/> with the provided frame.
    /// </summary>
    /// <param name="host">The frame to use for navigation.</param>
    public NavigationHost(Frame host)
    {
        Host = host;
    }

    /// <inheritdoc/>
    protected override IObservable<object> PlatformNavigate()
    {
        var host = Host;

        _ = host.Navigate(InitializeView());
        SetViewModel(host.Content);

        return Observable.Return(host.Content);
    }

    /// <inheritdoc/>
    protected override IObservable<object?> PlatformGoBack()
    {
        var host = Host;

        host.GoBack();
        SetViewModel(host.Content as IViewFor);

        return Observable.Return(host.Content);
    }
}
