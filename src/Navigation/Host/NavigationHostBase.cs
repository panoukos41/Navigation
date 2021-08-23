using Flurl;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace P41.Navigation.Host;

/// <summary>
/// A base implementation for the <see cref="INavigationHost"/>.
/// </summary>
public abstract class NavigationHostBase : INavigationHost
{
    private NavigationStack _stack = null!;
    private readonly Subject<INavigationHost> _whenNavigating = new();

    /// <inheritdoc/>
    public int Count => Stack.Count;

    /// <inheritdoc/>
    public IViewFor? CurrentView { get; private set; }

    /// <inheritdoc/>
    public Url? CurrentRequest => Stack.Count is 0 ? null : Stack.Peek();

    /// <inheritdoc/>
    public Interaction<Url, Unit> Push { get; } = new();

    /// <inheritdoc/>
    public Interaction<Unit, Url?> Pop { get; } = new();

    /// <inheritdoc/>
    public Interaction<Unit, bool> ShouldPopRoot { get; } = new();

    /// <inheritdoc/>
    public IObservable<INavigationHost> WhenNavigating => _whenNavigating.AsObservable();

    /// <summary>
    /// The navigation stack.
    /// </summary>
    protected NavigationStack Stack
    {
        get => _stack;
        set
        {
            _stack = value;
            Stack.Change.Subscribe(_ => _whenNavigating.OnNext(this));
        }
    }

    /// <summary>
    /// Initialization of navigation logic.
    /// </summary>
    protected NavigationHostBase()
    {
        Stack = new();
        Push.RegisterHandler(async (input, handled) =>
        {
            if (CurrentRequest == input) return Unit.Default;

            NavigatingFromViewModel();

            Stack.Push(input);
            CurrentView = await PlatformNavigate();

            SetViewModel();

            NavigatedToViewModel();

            return Unit.Default;
        });

        Pop.RegisterHandler(async (input, handled) =>
        {
            if (Stack.Count == 0) throw new InvalidOperationException("There is nothing to pop.");

            if (Stack.Count > 1 || await ShouldPopRoot.Handle(Unit.Default))
            {
                NavigatingFromViewModel();

                CurrentView = await PlatformGoBack();
                var popped = Stack.Pop();

                SetViewModel();

                NavigatedToViewModel();

                return popped;
            }

            return CurrentRequest;
        });

        ShouldPopRoot.RegisterHandler(static c => c.SetOutput(false));
    }

    private void NavigatedToViewModel()
    {
        // Going to the View/ViewModel
        if (CurrentView?.ViewModel is INavigationAware nextVm)
        {
            nextVm.NavigatedTo(CurrentRequest!, this).Subscribe();
        }
    }
    private void NavigatingFromViewModel()
    {
        // Leaving the View/ViewModel
        if (CurrentView?.ViewModel is INavigationAware previusVm)
        {
            previusVm.NavigatingFrom().Subscribe();
        }
    }

    private void SetViewModel()
    {
        // Only set it if not set since some times
        // it can be set on the platfrom navigate like android.
        if (CurrentView is { ViewModel: null })
        {
            CurrentView.ViewModel = InitializeViewModel();
        }
    }

    /// <summary>
    /// This method is called by the implementation.
    /// </summary>
    /// <returns>The new <see cref="CurrentView"/> object.</returns>
    protected abstract IObservable<IViewFor> PlatformNavigate();

    /// <summary>
    /// This method is called by the implementation.
    /// </summary>
    /// <returns>The new <see cref="CurrentView"/> object.</returns>
    /// <remarks>If we pop the root page null should be returned.</remarks>
    protected abstract IObservable<IViewFor?> PlatformGoBack();

    /// <summary>
    /// Initialize a new ViewModel for the CurrentRequest.
    /// </summary>
    protected abstract object? InitializeViewModel();
}

/// <summary>
/// A base implementation for the <see cref="INavigationHost"/> that
/// takes into considertaion View creation and the Host that will host them.
/// </summary>
/// <typeparam name="THost">The type of the host.</typeparam>
/// <typeparam name="TView">The type of the hosted views.</typeparam>
public abstract class NavigationHostBase<THost, TView> : NavigationHostBase
{
    private THost _host = default!;

    /// <summary>
    /// Gets or sets the Host that should be used for the navigation.
    /// Do not leave this null when using the host.
    /// </summary>
    public THost Host
    {
        get => _host ?? throw new ArgumentNullException(nameof(Host), "A Host was not provided for navigation.");
        set => _host = value ?? throw new NullReferenceException("You tried to set a null host.");
    }

    /// <summary>
    /// Initialize a new View for the CurrentRequest.
    /// </summary>
    protected abstract TView InitializeView();
}
