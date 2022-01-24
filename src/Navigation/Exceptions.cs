using System;

namespace P41.Navigation.Exceptions;

/// <summary>
/// Exception thrown when a route is not matched to any mapping.
/// </summary>
public class CouldNotNavigateException : Exception
{
    /// <summary>
    /// Initialize a new <see cref="CouldNotNavigateException"/>.
    /// </summary>
    public CouldNotNavigateException(string route) : base($"Could not navigate to '{route}'.")
    {
    }
}
