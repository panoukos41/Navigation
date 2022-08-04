using Flurl;
using System.Collections.Generic;

namespace P41.Navigation.Host;

/// <summary>
/// Wrapper class for a <see cref="Url"/> to access segments and parameters faster.
/// </summary>
public class NavigationParameters
{
    private readonly Url _url;

    /// <summary>
    /// The raw query of the <see cref="Url"/>.
    /// </summary>
    public QueryParamCollection Query => _url.QueryParams;

    /// <summary>
    /// The raw segments of the <see cref="Url"/>.
    /// </summary>
    public IList<string> Segments => _url.PathSegments;

    /// <summary>
    /// Initialize a new instance of <see cref="NavigationParameters"/> class.
    /// </summary>
    /// <param name="url"></param>
    public NavigationParameters(Url url)
    {
        _url = url;
    }

    /// <summary>
    /// Access the quert parameters by key.
    /// </summary>
    /// <param name="key">The query parameter to access.</param>
    /// <returns>The value of a query parameter or null if it does not exist.</returns>
    public string? this[string key] => _url.QueryParams.TryGetFirst(key, out var value) ? value.ToString() : null;

    /// <summary>
    /// Access the path segemnts by index.
    /// </summary>
    /// <param name="index">The index of the segment to return starting by 0.</param>
    /// <returns>The requested segment or null if it does not exist.</returns>
    public string? this[int index] => _url.PathSegments.Count > index + 1 ? _url.PathSegments[index] : null;

    /// <summary>
    /// Returns a string that represents the request.
    /// </summary>
    /// <returns>A string that represents the request.</returns>
    public override string ToString() => _url;
}
