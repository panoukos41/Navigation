using System;
using System.Reactive;
using System.Threading.Tasks;

namespace ReactiveUI;

/// <summary>
/// Internal extension methods for ReactiveUI Interactions.
/// </summary>
internal static class InteractionEx
{
    /// <summary>
    /// Registers a synchronous interaction handler, that will always
    /// set the output to the returned value.
    /// </summary>
    /// <typeparam name="TInput">The input type.</typeparam>
    /// <typeparam name="TOutput">The output type.</typeparam>
    /// <param name="interaction">The interaction to register the handler on.</param>
    /// <param name="handler">A function that always returns an output.</param>
    /// <returns>A disposable which, when disposed, will unregister the handler.</returns>
    /// <remarks>
    /// This overload of RegisterHandler is only useful if the handler can handle the
    /// interaction immediately. That is, it does not need to wait for the user or some
    /// other collaborating component.
    /// </remarks>
    public static IDisposable RegisterHandler<TInput, TOutput>(
        this Interaction<TInput, TOutput> interaction,
        Func<TInput, bool, TOutput> handler)
    {
        return interaction.RegisterHandler(c => c.SetOutput(handler(c.Input, c.IsHandled)));
    }

    /// <summary>
    /// Registers a task-based asynchronous interaction handler that will always
    /// set the output to the returned value.
    /// </summary>
    /// <typeparam name="TInput">The input type.</typeparam>
    /// <typeparam name="TOutput">The output type.</typeparam>
    /// <param name="interaction">The interaction to register the handler on.</param>
    /// <param name="handler">A function that always returns an output.</param>
    /// <returns>A disposable which, when disposed, will unregister the handler.</returns>
    /// <remarks>
    /// This overload of RegisterHandler is useful if the handler needs to perform some
    /// asynchronous operation, such as displaying a dialog and waiting for the user's response.
    /// </remarks>
    public static IDisposable RegisterHandler<TInput, TOutput>(
        this Interaction<TInput, TOutput> interaction,
        Func<TInput, bool, Task<TOutput>> handler)
    {
        return interaction.RegisterHandler(async c => c.SetOutput(await handler(c.Input, c.IsHandled)));
    }

    /// <summary>
    /// Set the input to <see cref="Unit.Default"/>.
    /// </summary>
    /// <typeparam name="TOutput">The type of the return parameter.</typeparam>
    /// <param name="interaction">The interaction.</param>
    /// <returns>An observable taht ticks when the interaction completes.</returns>
    public static IObservable<TOutput> Handle<TOutput>(this Interaction<Unit, TOutput> interaction) =>
        interaction.Handle(Unit.Default);

    /// <summary>
    /// Set the output to <see cref="Unit.Default"/>.
    /// </summary>
    /// <typeparam name="TInput">The type of the input parameter.</typeparam>
    /// <param name="interaction">The interaction.</param>
    public static void SetOutput<TInput>(this InteractionContext<TInput, Unit> interaction) =>
        interaction.SetOutput(Unit.Default);
}
