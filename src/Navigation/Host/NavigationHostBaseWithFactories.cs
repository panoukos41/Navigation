using System;
using System.Collections.Generic;

namespace P41.Navigation.Host;

/// <summary>
/// Base implementation from which all platform implementations derive.
/// It contains dictionaries with factory methods and already overrides
/// InitliazeViewmodel and InitializeView to use the factory methods.
/// </summary>
/// <typeparam name="THost">The type of the host.</typeparam>
/// <typeparam name="TView">The type of the stored view.</typeparam>
/// <typeparam name="TImplementation">The type inheriting this base.</typeparam>
public abstract class NavigationHostBaseWithFactories<THost, TView, TImplementation> : NavigationHostBase<THost, TView>
    where TImplementation : NavigationHostBaseWithFactories<THost, TView, TImplementation>
{
    /// <summary>
    /// A page key,View factory function to get a View for the host.
    /// </summary>
    private Dictionary<string, Func<TView>> Views { get; } = new();

    /// <summary>
    /// A page key, ViewModel factory function to get a ViewModel for the key.
    /// </summary>
    private Dictionary<string, Func<object?>> ViewModels { get; } = new();

    /// <inheritdoc/>
    protected override TView InitializeView()
    {
        // todo: Page initialization logic.

        return Views[page].Invoke();
    }

    /// <inheritdoc/>
    protected override object? InitializeViewModel()
    {
        // todo: ViewModel initialization logic.
        return ViewModels.TryGetValue(page, out var viewModel) ? viewModel.Invoke() : null;
    }

    /// <summary>
    /// Add a PageKey - View factory pair.
    /// </summary>
    /// <param name="pageKey">The page key.</param>
    /// <param name="factory">A factory method to generate the view for the pagekey.</param>
    /// <returns>The host for further configuration.</returns>
    /// <remarks>This can override the key value if called more than once.</remarks>
    public TImplementation AddView(
        string pageKey, Func<TView> factory)
    {
        Views[pageKey] = factory;
        return (TImplementation)this;
    }

    /// <summary>
    /// Add a PageKey - ViewModel factory pair.
    /// </summary>
    /// <param name="pageKey">The page key.</param>
    /// <param name="factory">A factory method to generate the viewmodel for the pagekey.</param>
    /// <returns>The host for further configuration.</returns>
    /// <remarks>This can override the key value if called more than once.</remarks>
    public TImplementation AddViewModel(
        string pageKey, Func<object?> factory)
    {
        ViewModels[pageKey] = factory;
        return (TImplementation)this;
    }

    /// <summary>
    /// Using the <see cref="AddView(string, Func{TView})"/>
    /// and <see cref="AddViewModel(string, Func{object?})"/>
    /// methods register both factories for the PageKey.
    /// </summary>
    /// <param name="pageKey">The page key.</param>
    /// <param name="viewFactory">A factory method to generate the view for the pagekey.</param>
    /// <param name="viewModelFactory">A factory method to generate the viewmodel for the pagekey.</param>
    /// <returns>The host for further configuration.</returns>
    /// <remarks>This can override the key value if called more than once.</remarks>
    public TImplementation AddPair(
        string pageKey, Func<TView> viewFactory, Func<object?> viewModelFactory)
    {
        AddView(pageKey, viewFactory);
        AddViewModel(pageKey, viewModelFactory);
        return (TImplementation)this;
    }
}
