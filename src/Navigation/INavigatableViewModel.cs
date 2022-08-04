using Flurl;
using P41.Navigation.Host;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading;

// Interface and class to help with ViewModel navigation.
// The interface/helper class/extensions are inspired and derived from
// the excelent work done on ReactiveUI (https://github.com/reactiveui/reactiveui)
// so that this project can offer a similar approach to navigation.

namespace P41.Navigation;

/// <summary>
/// Interface implemented by ViewModels to receive navigation events.
/// </summary>
public interface INavigatableViewModel
{
    /// <summary>
    /// Navigator class that will receive navigation events.
    /// </summary>
    ViewModelNavigator Navigator { get; }
}

/// <summary>
/// Helper class that you instantiate in your ViewModel.
/// Hosts will call this class when the view is NavigatedTo and From.
/// </summary>
public sealed class ViewModelNavigator : IDisposable
{
    private readonly List<Func<NavigationParameters, IEnumerable<IDisposable>>> _blocks;

    private readonly Subject<NavigationParameters> _navigatedTo;

    private readonly Subject<Unit> _navigatingFrom;

    private IDisposable _navigationHandle = Disposable.Empty;

    private int _refCount;

    /// <summary>
    /// The <see cref="INavigationHost"/> associated with this Navigator,
    /// this is set by the host itself and can be used for further navigation.
    /// </summary>
    public INavigationHost? Host { get; internal set; }

    /// <summary>
    /// Gets an observable which will tick every time the Navigator is navigated to.
    /// </summary>
    public IObservable<NavigationParameters> WhenNavigatedTo => _navigatedTo;

    /// <summary>
    /// Gets an observable which will tick every time the Navigator is navigating from.
    /// </summary>
    public IObservable<Unit> WhenNavigatingFrom => _navigatingFrom;

    /// <summary>
    /// Initializes a new instance of <see cref="ViewModelNavigator"/> class.
    /// </summary>
    public ViewModelNavigator()
    {
        _blocks = new List<Func<NavigationParameters, IEnumerable<IDisposable>>>();
        _navigatedTo = new Subject<NavigationParameters>();
        _navigatingFrom = new Subject<Unit>();
    }

    /// <summary>
    /// This method is called when a View is navigated to.
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns>A Disposable that calls NavigatingFrom when disposed.</returns>
    public IDisposable NavigatedTo(NavigationParameters parameters)
    {
        if (Interlocked.Increment(ref _refCount) == 1)
        {
            var value = new CompositeDisposable(_blocks.SelectMany(x => x(parameters)));
            Interlocked.Exchange(ref _navigationHandle, value).Dispose();
            _navigatedTo.OnNext(parameters);
        }

        return Disposable.Create(delegate
        {
            NavigatingFrom();
        });
    }

    /// <summary>
    /// This method is called when a view is navigating to another view.
    /// </summary>
    /// <param name="ignoreRefCount"></param>
    public void NavigatingFrom(bool ignoreRefCount = false)
    {
        if (Interlocked.Decrement(ref _refCount) == 0 || ignoreRefCount)
        {
            Interlocked.Exchange(ref _navigationHandle, Disposable.Empty).Dispose();
            _navigatingFrom.OnNext(Unit.Default);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _navigationHandle.Dispose();
        _navigatedTo.Dispose();
        _navigatingFrom.Dispose();
    }

    /// <summary>
    /// Adds a action blocks to the list of registered blocks.
    /// These will called on NavigatedTo, then disposed on NavigatingFrom.
    /// </summary>
    /// <param name="block">The block to add.</param>
    internal void AddNavigationBlock(Func<NavigationParameters, IEnumerable<IDisposable>> block)
    {
        _blocks.Add(block);
    }
}

/// <summary>
/// Extension methods that help with <see cref="ViewModelNavigator"/> class.
/// </summary>
public static class NavigatorExtensions
{
    /// <summary>
    /// Setup logic to be executed when a ViewModel is navigated to.
    /// Add disposables to the <see cref="CompositeDisposable"/> to be notified
    /// when you are navigated away from the ViewModel.
    /// </summary>
    public static void WhenNavigatedTo(this INavigatableViewModel item, Action<NavigationParameters, CompositeDisposable> block)
    {
        Action<NavigationParameters, CompositeDisposable> block2 = block;
        if (item is null) throw new ArgumentNullException("item");

        item.Navigator.AddNavigationBlock(url =>
        {
            CompositeDisposable compositeDisposable = new CompositeDisposable();
            block2(url, compositeDisposable);
            return new[] { compositeDisposable };
        });
    }

    /// <summary>
    /// Navigate to the <paramref name="url"/> destination.
    /// </summary>
    public static void Navigate(this INavigatableViewModel item, Url url)
    {
        if (item is null) throw new ArgumentNullException("item");
        if (item.Navigator.Host is null) throw new ArgumentNullException("Host", "The Navigator Host property is null.");

        item.Navigator.Host.Navigate(url);
    }

    /// <summary>
    /// Request to navigate back.
    /// </summary>
    public static void GoBack(this INavigatableViewModel item)
    {
        if (item is null) throw new ArgumentNullException("item");
        if (item.Navigator.Host is null) throw new ArgumentNullException("Host", "The Navigator Host property is null.");

        item.Navigator.Host.GoBack();
    }
}

/// <summary>
/// Extension methods for <see cref="IViewFor{T}"/> imlementations.
/// </summary>
public static class IViewForExtensions
{
    /// <summary>
    /// Navigate to the <paramref name="url"/> destination.
    /// </summary>
    public static void Navigate<T>(this IViewFor<T> view, Url url)
        where T : class, INavigatableViewModel
    {
        if (view is null) throw new ArgumentNullException("view");
        if (view.ViewModel is null) throw new ArgumentNullException("view.ViewModel");
        if (view.ViewModel.Navigator.Host is null) throw new ArgumentNullException("view.ViewModel.Navigator.Host", "The Navigator Host property is null.");

        view.ViewModel.Navigator.Host.Navigate(url);
    }

    /// <summary>
    /// Request to navigate back.
    /// </summary>
    public static void GoBack<T>(this IViewFor<T> view, Url url)
        where T : class, INavigatableViewModel
    {
        if (view is null) throw new ArgumentNullException("view");
        if (view.ViewModel is null) throw new ArgumentNullException("view.ViewModel");
        if (view.ViewModel.Navigator.Host is null) throw new ArgumentNullException("view.ViewModel.Navigator.Host", "The Navigator Host property is null.");

        view.ViewModel.Navigator.Host.GoBack();
    }
}
