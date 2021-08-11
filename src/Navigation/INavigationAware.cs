using System;
using System.Reactive;

namespace P41.Navigation
{
    /// <summary>
    /// Inetrface implemented by ViewModels to receive navigation events.
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Method executed when a ViewModel is navigated to.
        /// This is executed for forward and backward navigation.
        /// </summary>
        /// <param name="parameters">The parameters passed to the navigation host for this object.</param>
        /// <returns></returns>
        IObservable<Unit> NavigatedTo(NavigationRequest parameters);

        /// <summary>
        /// Method executed when a Viewmodel is being navigating away.
        /// This is executed for forward and backward navigation.
        /// </summary>
        /// <returns></returns>
        IObservable<Unit> NavigatingFrom();
    }
}
