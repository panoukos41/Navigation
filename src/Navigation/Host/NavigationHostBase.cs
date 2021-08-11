using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace P41.Navigation.Host
{
    /// <summary>
    /// A base implementation for navigation hosts. You only need to implement
    /// native navigation and viewmodel creation, everything else is handled
    /// by the base class.
    /// </summary>
    public abstract class NavigationHostBase : INavigationHost
    {
        private readonly Subject<INavigationHost> _hostSubject = new();

        /// <inheritdoc/>
        public int Count => Stack.Count;

        /// <inheritdoc/>
        public IViewFor? CurrentView { get; private set; }

        /// <inheritdoc/>
        public NavigationRequest? CurrentRequest => Stack.Count is 0 ? null : Stack.Peek();

        /// <inheritdoc/>
        public Interaction<NavigationRequest, Unit> Push { get; } = new();

        /// <inheritdoc/>
        public Interaction<Unit, NavigationRequest?> Pop { get; } = new();

        /// <inheritdoc/>
        public Interaction<Unit, bool> ShouldPopRoot { get; } = new();

        /// <inheritdoc/>
        public IObservable<INavigationHost> WhenNavigated => _hostSubject.AsObservable();

        /// <summary>
        /// The navigation stack.
        /// </summary>
        protected NavigationStack Stack { get; set; } = new();

        /// <summary>
        /// Initialization of navigation logic.
        /// </summary>
        protected NavigationHostBase()
        {
            Stack.Change.Subscribe(_ => _hostSubject.OnNext(this));
            Push.RegisterHandler(async (input, handled) =>
            {
                if (CurrentRequest == input) return Unit.Default;

                if (Stack.Count == 0) input.IsRoot = true;

                NavigatingFromViewModel();

                CurrentView = await PlatformNavigate(input);
                Stack.Push(input);

                var nextVm = InitializeViewModel(input.Page);
                if (nextVm is { })
                {
                    CurrentView.ViewModel = nextVm;
                }

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

                    if (CurrentView.ViewModel is null)
                    {
                        CurrentView.ViewModel = InitializeViewModel(CurrentRequest!.Page);
                    }

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
                nextVm.NavigatedTo(CurrentRequest!).Subscribe();
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

        /// <summary>
        /// This method is called by the implementation.
        /// </summary>
        /// <param name="request">The navigation request to execute.</param>
        /// <returns>The new <see cref="CurrentView"/> object.</returns>
        protected abstract IObservable<IViewFor> PlatformNavigate(NavigationRequest request);

        /// <summary>
        /// This method is called by the implementation.
        /// </summary>
        /// <returns>The new <see cref="CurrentView"/> object.</returns>
        /// <remarks>If we pop the root page null should be returned.</remarks>
        protected abstract IObservable<IViewFor?> PlatformGoBack();

        /// <summary>
        /// Initialize a new ViewModel for the specified page key.
        /// </summary>
        protected abstract object? InitializeViewModel(string page);
    }

    /// <summary>
    /// A base implementation for navigation hosts.
    /// </summary>
    /// <typeparam name="THost"></typeparam>
    /// <typeparam name="TView"></typeparam>
    public abstract class NavigationHostBase<THost, TView> : NavigationHostBase
    {
        private THost _host = default!;

        /// <summary>
        /// Gets or sets the Frame that should be used for the navigation.
        /// Do not leave this null when using the host.
        /// </summary>
        public THost Host
        {
            get => _host ?? throw new ArgumentNullException(nameof(Host), "A Host was not provided for navigation.");
            set => _host = value ?? throw new NullReferenceException("You tried to set a null host.");
        }

        /// <summary>
        /// Initialize a new View for the specified page key.
        /// </summary>
        protected abstract TView InitializeView(string page);
    }
}
