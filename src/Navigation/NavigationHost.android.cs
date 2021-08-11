using AndroidX.Fragment.App;
using P41.Navigation.Host;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace P41.Navigation
{
    public class NavigationHost : NavigationHostBaseWithFactories<FragmentManager, Fragment, NavigationHost>
    {
        private int fragmentContainerId;
        private const string BACKSTACK_TAG = "fragment_number_";

        /// <summary>
        /// Gets or sets the FragmentContainerId that should be used for the navigation.
        /// </summary>
        public virtual int FragmentContainerId
        {
            get => fragmentContainerId != 0
                ? fragmentContainerId
                : throw new NullReferenceException($"{nameof(FragmentContainerId)} was not set! Please set it before using the service.");
            set => fragmentContainerId = value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NavigationHost"/>.
        /// </summary>
        public NavigationHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NavigationHost"/> with
        /// values for the containerId and the fragmentmanager.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="fragmentContainerId"></param>
        public NavigationHost(FragmentManager manager, int fragmentContainerId)
        {
            Host = manager;
            FragmentContainerId = fragmentContainerId;
        }

        /// <summary>
        /// </summary>
        /// <param name="fragmentManager"></param>
        /// <returns></returns>
        public virtual NavigationHost SetFragmentManager(FragmentManager fragmentManager)
        {
            Host = fragmentManager;
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="fragmentContainerId"></param>
        /// <returns></returns>
        public virtual NavigationHost SetFragmentContainerId(int fragmentContainerId)
        {
            FragmentContainerId = fragmentContainerId;
            return this;
        }

        /// <inheritdoc/>
        protected override IObservable<IViewFor> PlatformNavigate(NavigationRequest request)
        {
            var manager = Host;

            var fragment = Views[request.Page].Invoke();
            var index = manager.BackStackEntryCount - 1;
            var key = $"{index}: {request}";

            manager.BeginTransaction()
                .AddToBackStack(key)
                .Replace(FragmentContainerId, fragment, key)
                .Commit();

            return GetHostContent(manager);
        }

        /// <inheritdoc/>
        protected override IObservable<IViewFor?> PlatformGoBack()
        {
            var manager = Host;

            manager.PopBackStack();

            return GetHostContent(manager);
        }

        private static IObservable<IViewFor> GetHostContent(FragmentManager manager)
        {
            var last = manager.BackStackEntryCount - 1;

            return manager.Fragments[last] is IViewFor view
                ? Observable.Return(view)
                : throw new ArgumentException($"View must implement {nameof(IViewFor)}");
        }
    }
}
