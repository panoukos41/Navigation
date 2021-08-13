using ReactiveUI;
using System;
using System.Reactive;

namespace P41.Navigation
{
    /// <summary>
    /// A Host for navigation eg: Frame for UAP or FragmentHost for Android.
    /// The host can navigate between views and execute navigation methods
    /// on their ViewModels that implement <see cref="INavigationAware"/>.
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
        IViewFor? CurrentView { get; }

        /// <summary>
        /// The current active page/parameters pair. If nothing has been pushed
        /// or the root has been popped then this value will be null.
        /// </summary>
        NavigationRequest? CurrentRequest { get; }

        /// <summary>
        /// An observable that signals a navigation (push or pop) happened
        /// and passes the itself for further processing.
        /// </summary>
        IObservable<INavigationHost> WhenNavigated { get; }

        /// <summary>
        /// Intercation to determine if the host should push,
        /// if the interaction is handled then it won't push.
        /// </summary>
        /// <remarks>You can also register a handler.
        /// If the interaction is handled then it won't push.<br />
        /// </remarks>
        Interaction<NavigationRequest, Unit> Push { get; }

        /// <summary>
        /// Pop the navigation
        /// 
        /// </summary>
        /// <remarks>You can also register a handler.
        /// If the interaction is handled then it won't pop.<br />
        /// When you hande it yourself you should set
        /// <see langword="null"/> or <see cref="CurrentRequest"/><br />
        /// but in the end it's up to you to decide.
        /// </remarks>
        Interaction<Unit, NavigationRequest?> Pop { get; }

        /// <summary>
        /// 
        /// </summary>
        Interaction<Unit, bool> ShouldPopRoot { get; }
    }


    /// <summary>
    /// Extension methods for the <see cref="INavigationHost"/> interface.
    /// </summary>
    public static class INavigationHostEx
    {
        /// <summary>
        /// Get the current view as <see cref="IViewFor{T}"/> this
        /// can throw an <see cref="InvalidCastException"/>.
        /// </summary>
        /// <typeparam name="T">The ViewModel type.</typeparam>
        /// <param name="host">The current host.</param>
        /// <exception cref="InvalidCastException">When the View can't be cast.</exception>
        /// <remarks>Null is returned only when the current view is null not when the cast fails.</remarks>
        public static IViewFor<T>? GetCurrentView<T>(this INavigationHost host) where T : class =>
            host.CurrentView is null ? null : (IViewFor<T>)host.CurrentView;

        /// <summary>
        /// Get the ViewModel property of a CurrentView.
        /// </summary>
        /// <param name="host">The current host.</param>
        public static object? GetViewModel(this INavigationHost host) =>
            host.CurrentView?.ViewModel;

        /// <summary>
        /// Get the ViewModel property of CurrentView as <typeparamref name="T"/>
        /// this uses <see cref="GetCurrentView{T}(INavigationHost)"/> and can throw
        /// an <see cref="InvalidCastException"/>.
        /// </summary>
        /// <typeparam name="T">The ViewModel type.</typeparam>
        /// <param name="host">The current host.</param>
        /// <exception cref="InvalidCastException">When the View can't be cast.</exception>
        public static T? GetViewModel<T>(this INavigationHost host) where T : class =>
            host.GetCurrentView<T>()?.ViewModel;

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
        /// get an IObservable that when subscribed executes the command.
        /// </summary>
        /// <param name="host">The current host.</param>
        /// <param name="request">The page and any parameters to navigate to.</param>
        /// <returns>Ab IObservable that when subscribed executes the command.</returns>
        public static IObservable<Unit> Push(this INavigationHost host, NavigationRequest request)
        {
            return host.Push.Handle(request);
        }

        /// <summary>
        /// Push a new page/parameters pair to the stack and
        /// get an IObservable that when subscribed executes the command.
        /// </summary>
        /// <param name="host">The current host.</param>
        /// <param name="page">The page to navigate to.</param>
        /// <returns>Ab IObservable that when subscribed executes the command.</returns>
        public static IObservable<Unit> Push(this INavigationHost host, string page)
        {
            return host.Push(new NavigationRequest(page));
        }

        /// <summary>
        /// Push a new page/parameters pair to the stack and
        /// return an IDisposable that when disposed you usubscribe from the event.
        /// </summary>
        /// <param name="host">The current host.</param>
        /// <param name="request">The page and any parameters to navigate to.</param>
        /// <returns>An IDisposable to unsubscribe from the event.</returns>
        public static IDisposable Navigate(this INavigationHost host, NavigationRequest request)
        {
            return host.Push.Handle(request).Subscribe();
        }

        /// <summary>
        /// Push a new page/parameters pair to the stack and
        /// return an IDisposable that when disposed you usubscribe from the event.
        /// </summary>
        /// <param name="host">The current host.</param>
        /// <param name="page">The page to navigate to.</param>
        /// <returns>An IDisposable to unsubscribe from the event.</returns>
        public static IDisposable Navigate(this INavigationHost host, string page)
        {
            return host.Navigate(new NavigationRequest(page));
        }

        /// <summary>
        /// Pop the current page/parameters pair from the stack and return it.
        /// </summary>
        /// <param name="host">The current host.</param>
        /// <returns>The item that was removed from the stack.</returns>
        public static IObservable<NavigationRequest?> Pop(this INavigationHost host)
        {
            return host.Pop.Handle();
        }

        /// <summary>
        /// Decide if you can go back depending on the page count.
        /// </summary>
        /// <param name="host">The current host.</param>
        /// <param name="includeRoot">Wheter the root should be counted as a page that you can go back.</param>
        /// <returns>An IDisposable to unsubscribe from the event.</returns>
        public static bool CanGoBack(this INavigationHost host, bool includeRoot = false)
        {
            return includeRoot
                ? host.Count > 0
                : host.Count > 1;
        }

        /// <summary>
        /// Pop the current page/parameters pair from the stack and
        /// return an IDisposable that when disposed you usubscribe from the event.
        /// </summary>
        /// <param name="host">The current host.</param>
        /// <returns>An IDisposable to unsubscribe from the event.</returns>
        public static IDisposable GoBack(this INavigationHost host)
        {
            return host.Pop.Handle().Subscribe();
        }

        /// <summary>
        /// Deconstruct a <see cref="INavigationHost"/>.
        /// </summary>
        /// <param name="host">The host to deconstruct.</param>
        /// <param name="currentView">The current View.</param>
        /// <param name="currentRequest">The current ViewModel.</param>
        public static void Deconstruct(this INavigationHost host, out IViewFor? currentView, out NavigationRequest? currentRequest)
        {
            currentView = host.CurrentView;
            currentRequest = host.CurrentRequest;
        }
    }
}
