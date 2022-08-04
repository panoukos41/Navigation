using Flurl;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace P41.Navigation.Host;

/// <summary>
/// Helper Dictionary class to store mappings.
/// </summary>
/// <typeparam name="TView">The type of the view stored.</typeparam>
internal class Mappings<TView> : Dictionary<NavigationRoute, Func<TView>>
    where TView : class
{
    public Func<TView> Match(Url route)
    {
        try
        {
            return this.First(r => r.Key.Match(route)).Value;
        }
        catch
        {
            throw new Exceptions.CouldNotNavigateException(route);
        }
    }
}

/// <summary>
/// A base implementation for the <see cref="INavigationHost"/>.
/// </summary>
/// <typeparam name="TView">The type of the view used for navigation.</typeparam>
/// <typeparam name="TImpliments">The type of the object that inherits this class.</typeparam>
public abstract class NavigationHostBase<TView, TImpliments> : INavigationHost
    where TView : class
    where TImpliments : NavigationHostBase<TView, TImpliments>
{
    private readonly Subject<Unit> _rootPopped = new();
    private readonly Subject<INavigationHost> _navigated = new();
    private Mappings<TView> _mappings = new();

    /// <inheritdoc/>
    public int Count => Stack.Count;

    /// <inheritdoc/>
    public object? CurrentView { get; private set; }

    /// <inheritdoc/>
    public Url? CurrentRequest => Stack.Count is 0 ? null : Stack.Peek();

    /// <inheritdoc/>
    public Interaction<Url, Unit> Push { get; } = new();

    /// <inheritdoc/>
    public Interaction<Unit, Unit> Pop { get; } = new();

    /// <summary>
    /// Function to determine if the root page should be popped.
    /// </summary>
    public Func<bool> ShouldPopRoot { get; set; }

    /// <inheritdoc/>
    public IObservable<INavigationHost> WhenNavigated => _navigated.AsObservable();

    /// <summary>
    /// An observable that signals the root has been popped.
    /// </summary>
    public IObservable<Unit> WhenRootPopped => _rootPopped.AsObservable();

    /// <summary>
    /// The underlying navigation stack.
    /// </summary>
    protected Stack<Url> Stack { get; set; }

    /// <summary>
    /// Initialization of navigation logic.
    /// </summary>
    protected NavigationHostBase()
    {
        Stack = new();
        Push.RegisterHandler(PushExecute);
        Pop.RegisterHandler(PopExecute);
        ShouldPopRoot = static () => false;
    }

    /// <summary>
    /// Map routes to views.
    /// </summary>
    public TImpliments Map(NavigationRoute route, Func<TView> viewFactory)
    {
        _mappings[route] = viewFactory;
        return (TImpliments)this;
    }

    /// <summary>
    /// Get an array of the registered mappings.
    /// </summary>
    public NavigationRoute[] GetMappings() => _mappings.Keys.ToArray();

    private void PushExecute(InteractionContext<Url, Unit> context)
    {
        var request = context.Input;

        if (CurrentRequest == request)
        {
            context.SetOutput(Unit.Default);
            return;
        }

        NavigatingFromViewModel();

        Stack.Push(request);

        var view = _mappings.Match(request).Invoke();

        CurrentView = PlatformNavigate(view);

        NavigatedToViewModel();
        _navigated.OnNext(this);

        context.SetOutput(Unit.Default);
    }

    private void PopExecute(InteractionContext<Unit, Unit> context)
    {
        if (Stack.Count == 0) throw new InvalidOperationException("There is nothing to pop.");

        if (Stack.Count > 1 || ShouldPopRoot())
        {
            NavigatingFromViewModel();

            Stack.Pop();

            CurrentView = PlatformGoBack();

            NavigatedToViewModel();
            _navigated.OnNext(this);

            context.SetOutput(Unit.Default);

            if (Stack.Count == 0) _rootPopped.OnNext(Unit.Default);
            return;
        }
        context.SetOutput(Unit.Default);
    }

    /// <summary>
    /// Executes methods on ViewModel when it is navigated to.
    /// </summary>
    private void NavigatedToViewModel()
    {
        if (CurrentView is IViewFor { ViewModel: INavigatableViewModel nextVm })
        {
            var navigator = nextVm.Navigator;

            navigator.Host = this;
            navigator.NavigatedTo(new NavigationParameters(CurrentRequest!));
        }
    }

    /// <summary>
    /// Execute methods on ViewModel when it is navigated away.
    /// </summary>
    private void NavigatingFromViewModel()
    {
        if (CurrentView is IViewFor { ViewModel: INavigatableViewModel previusVm })
        {
            var navigator = previusVm.Navigator;

            navigator.Host = this;
            navigator.NavigatingFrom();
        }
    }

    /// <summary>
    /// Executes platform navigation logic and returns the active view.
    /// </summary>
    protected abstract object PlatformNavigate(TView view);

    /// <summary>
    /// Executes platform backwards navigation logic and returns the active view.
    /// </summary>
    protected abstract object? PlatformGoBack();
}

/// <inheritdoc/>
/// <typeparam name="THost">The type of the platform host.</typeparam>
/// <typeparam name="TView">The type of the view used for navigation.</typeparam>
/// <typeparam name="TImpliments">The type of the object that inherits this class.</typeparam>
public abstract class NavigationHostBase<THost, TView, TImpliments> : NavigationHostBase<TView, TImpliments>
    where TView : class
    where TImpliments : NavigationHostBase<TView, TImpliments>
{
    private THost? _host;

    /// <summary>
    /// The platform host.
    /// </summary>
    public THost Host
    {
        get => _host ?? throw new ArgumentNullException("Host");
        set => _host = value;
    }
}
