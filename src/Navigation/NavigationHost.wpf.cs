using P41.Navigation.Host;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

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
