using Flurl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace P41.Navigation.Host;

/// <summary>
/// A route to that specifies a navigation path.
/// </summary>
public class NavigationRoute : IEquatable<NavigationRoute?>
{
    /// <summary>
    /// Initialize a new <see cref="NavigationRoute"/> object.
    /// </summary>
    /// <param name="template"></param>
    public NavigationRoute(string template)
    {
        Template = template;
        Segments = template.Trim('/').Split('/');
        HasParameter = Segments.Last() == "{}";
    }

    /// <summary>
    /// The template that defines this route.
    /// </summary>
    public string Template { get; set; }

    /// <summary>
    /// Individual segments of the path.
    /// </summary>
    public IList<string> Segments { get; }

    /// <summary>
    /// If there exists one parameter.
    /// </summary>
    public bool HasParameter { get; }

    /// <summary>
    /// Match this route against a <see cref="Url"/> and see if it matches.
    /// </summary>
    /// <param name="request">The url to match against.</param>
    /// <returns>True when it matches otherwise false.</returns>
    public bool Match(Url request)
    {
        var segments = Segments;
        var requestSegments = request.PathSegments;

        if (segments.Count != requestSegments.Count) return false;

        for (int i = 0; i < segments.Count; i++)
        {
            var s = segments[i];
            var r = requestSegments[i];

            if (s == "{}") return true;
            if (s != r) return false;
        }
        return true;
    }

    /// <summary>
    /// Returns the <see cref="Template"/> property.
    /// </summary>
    public override string ToString()
    {
        return Template;
    }

    private static bool IsParameter(string segment) => segment.Equals("{}", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public static implicit operator string(NavigationRoute _) => _.ToString();

    /// <inheritdoc/>
    public static implicit operator NavigationRoute(string _) => new(_);

    #region Equals

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as NavigationRoute);
    }

    /// <inheritdoc/>
    public bool Equals(NavigationRoute? other)
    {
        return other != null && Template == other.Template;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Template);
    }

    /// <inheritdoc/>
    public static bool operator ==(NavigationRoute? left, NavigationRoute? right)
    {
        return EqualityComparer<NavigationRoute>.Default.Equals(left!, right!);
    }

    /// <inheritdoc/>
    public static bool operator !=(NavigationRoute? left, NavigationRoute? right)
    {
        return !(left == right);
    }

    #endregion
}
