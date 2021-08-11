using Microsoft.AspNetCore.Components;
using P41.Navigation.Host;
using ReactiveUI;
using System;

namespace P41.Navigation
{
    /// <summary>
    /// <see cref="INavigationHost"/> implementation.
    /// </summary>
    public class NavigationManagerHost : NavigationHostBase
    {
        private readonly NavigationManager nav;

        /// <summary>
        /// Initialzie a new <see cref="NavigationManagerHost"/>.
        /// </summary>
        /// <param name="nav"></param>
        public NavigationManagerHost(NavigationManager nav)
        {
            this.nav = nav;
        }

        // todo: Implement NavigationManager host.

        /// <inheritdoc/>
        protected override IObservable<IViewFor> PlatformNavigate(NavigationRequest request)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override IObservable<IViewFor?> PlatformGoBack()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override object? InitializeViewModel(string page)
        {
            return null;
        }
    }
}