using Flurl;
using System;
using System.Reactive;

namespace P41.Navigation;

/// <summary>
/// Inetrface implemented by ViewModels to receive navigation events.
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// Method executed when a ViewModel is navigated to.
    /// This is executed for forward and backward navigation.
    /// </summary>
    /// <param name="request">The request passed to the navigation host for this object.</param>
    /// <param name="host">The host on which the navigation happens.</param>
    /// <remarks>
    /// The <paramref name="request"/> is the same as the <paramref name="host"/>
    /// <see cref="INavigationHost.CurrentRequest"/> parameter.
    /// </remarks>
    IObservable<Unit> NavigatedTo(Url request, INavigationHost host);

    /// <summary>
    /// Method executed when a Viewmodel is being navigating away.
    /// This is executed for forward and backward navigation.
    /// </summary>
    IObservable<Unit> NavigatingFrom();
}
