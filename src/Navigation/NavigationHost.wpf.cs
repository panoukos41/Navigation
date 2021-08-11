using P41.Navigation.Host;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace P41.Navigation
{
    /// <summary>
    /// Implementation of <see cref="INavigationHost"/> that can be configured with factory
    /// methods for Views and ViewModels.
    /// </summary>
    public class NavigationHost : NavigationHostBaseWithFactories<Frame, Type, NavigationHost>
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
        protected override IObservable<IViewFor> PlatformNavigate(NavigationRequest request)
        {
            var host = Host;

            _ = host.Navigate(Views[request.Page].Invoke());

            return GetHostContent(host);
        }

        /// <inheritdoc/>
        protected override IObservable<IViewFor?> PlatformGoBack()
        {
            var host = Host;

            host.GoBack();

            return GetHostContent(host);
        }

        private static IObservable<IViewFor> GetHostContent(Frame host)
        {
            return host.Content is IViewFor view
                ? Observable.Return(view)
                : throw new ArgumentException($"View must implement {nameof(IViewFor)}");
        }
    }
}
