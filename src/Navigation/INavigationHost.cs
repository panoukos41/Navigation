using Flurl;
using ReactiveUI;
using System;
using System.Reactive;

namespace P41.Navigation;

/// <summary>
/// A Host for navigation eg: Frame for UAP or FragmentHost for Android.
/// The host can navigate between views and execute navigation methods
/// on their ViewModels that implement <see cref="INavigatableViewModel"/>.
/// </summary>
public interface INavigationHost
{
    /// <summary>
    /// The number of pages in the stack.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// The currently displayed view. If nothing has been pushed
    /// or the root has been popped then this value will be null.
    /// </summary>
    object? CurrentView { get; }

    /// <summary>
    /// The current active request. If nothing has been pushed
    /// or the root has been popped then this value will be null.
    /// </summary>
    Url? CurrentRequest { get; }

    /// <summary>
    /// An observable that signals a navigation (push or pop) has happened.
    /// </summary>
    IObservable<INavigationHost> WhenNavigated { get; }

    /// <summary>
    /// Interaction to push a new request. You can register
    /// a handler to determine if you want to push.
    /// </summary>
    Interaction<Url, Unit> Push { get; }

    /// <summary>
    /// Interaction to pop the current request. You can register
    /// a handler to determine if you want to pop.
    /// </summary>
    Interaction<Unit, Unit> Pop { get; }
}

/// <summary>
/// Extension methods for the <see cref="INavigationHost"/> interface.
/// </summary>
public static class INavigationHostExtensions
{
    /// <summary>
    /// Register a handler for the WhenNavigated event.
    /// </summary>
    /// <param name="host">The host register on.</param>
    /// <param name="onNext">A handler function.</param>
    /// <returns>An IDisposable that when disposed unsubscribes from the event.</returns>
    public static IDisposable WhenNavigated(this INavigationHost host, Action<INavigationHost> onNext)
    {
        return host.WhenNavigated.Subscribe(onNext);
    }

    /// <summary>
    /// Push a new page/parameters pair to the stack and
    /// return an IDisposable that when disposed you unsubscribe from the event.
    /// </summary>
    /// <param name="host">The current host.</param>
    /// <param name="request">The page and any parameters to navigate to.</param>
    /// <returns>An IDisposable to unsubscribe from the event.</returns>
    public static IDisposable Navigate(this INavigationHost host, Url request)
    {
        return host.Push.Handle(request).Subscribe();
    }

    /// <summary>
    /// Pop the current page/parameters pair from the stack and
    /// return an IDisposable that when disposed you unsubscribe from the event.
    /// </summary>
    /// <param name="host">The current host.</param>
    /// <returns>An IDisposable to unsubscribe from the event.</returns>
    public static IDisposable GoBack(this INavigationHost host)
    {
        return host.Pop.Handle(Unit.Default).Subscribe();
    }

    /// <summary>
    /// Deconstruct a <see cref="INavigationHost"/>.
    /// </summary>
    /// <param name="host">The host to deconstruct.</param>
    /// <param name="currentView">The current View.</param>
    /// <param name="currentRequest">The current ViewModel.</param>
    public static void Deconstruct(this INavigationHost host, out object? currentView, out Url? currentRequest)
    {
        currentView = host.CurrentView;
        currentRequest = host.CurrentRequest;
    }
}
