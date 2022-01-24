using Microsoft.UI.Xaml.Controls;
using P41.Navigation.Host;
using System;

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
    protected override object PlatformNavigate(Type view)
    {
        Host.Navigate(view);
        return Host.Content;
    }

    /// <inheritdoc/>
    protected override object? PlatformGoBack()
    {
        Host.GoBack();
        return Host.Content;
    }
}
