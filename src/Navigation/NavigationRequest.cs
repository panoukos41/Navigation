using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P41.Navigation
{
    /// <summary>
    /// A page - parameters request.
    /// </summary>
    public class NavigationRequest : IEquatable<NavigationRequest?>
    {
        /// <summary>
        /// Indicates wheter this is a root object eg: (the first entry in a stack).
        /// </summary>
        public bool IsRoot { get; internal set; }

        /// <summary>
        /// The name/key of the page to navigate to.
        /// </summary>
        public string Page { get; }

        /// <summary>
        /// A list of path values like URL path parameters.
        /// </summary>
        /// <value>eg: book/15/horror</value>
        public List<string> Paths { get; } = new();

        /// <summary>
        /// A dictionary of query values like URL query parameters.
        /// </summary>
        /// <value>eg: book?writer=George&amp;genre=Horror</value>
        /// <remarks>Values are not escaped by default.</remarks>
        public Dictionary<string, string> Queries { get; } = new();

        /// <summary>
        /// Initialize a new <see cref="NavigationRequest"/> instance.
        /// </summary>
        /// <param name="page">The name/key of the page to navigate to.</param>
        public NavigationRequest(string page)
        {
            Page = page;
        }

        /// <summary>
        /// Returns the url string for this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Paths.Count == 0 && Queries.Count == 0) return Page;

            var sb = new StringBuilder();

            sb.Append(Page);

            if (Paths.Count != 0)
            {
                sb.Append('/');
                //sb.AppendJoin('/', Paths);
                sb.Append(string.Join("/", Paths));
            }

            if (Queries.Count != 0)
            {
                sb.Append('?');
                //sb.AppendJoin('&', Queries.Select(pair => $"{pair.Key}={pair.Value}"));
                sb.Append(string.Join("&", Queries.Select(pair => $"{pair.Key}={pair.Value}")));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parse a string and convert it to <see cref="NavigationRequest"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static NavigationRequest Parse(string request)
        {
            ReadOnlySpan<char> span = request.AsSpan();
            ReadOnlySpan<char> queries;

            var index = span.IndexOf('?');

            var paths = index == -1
                ? span.ToString().Split('/')
                : span.Slice(0, index).ToString().Split('/');

            var newRequest = new NavigationRequest(paths[0]);

            foreach (var path in paths.Skip(1))
            {
                newRequest.Paths.Add(path);
            }

            if (index != -1)
            {
                queries = span.Slice(index + 1);

                foreach (var query in queries.ToString().Split('&'))
                {
                    var par = query.Split('=');
                    var key = par[0];
                    var val = par[1];
                    newRequest.Queries.Add(key, val);
                }
            }

            return newRequest;
        }

        #region Equals

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as NavigationRequest);
        }

        /// <inheritdoc/>
        public bool Equals(NavigationRequest? other)
        {
            return other is { }
                && other.ToString() == ToString();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(ToString());
        }

        /// <inheritdoc/>
        public static bool operator ==(NavigationRequest? left, NavigationRequest? right)
        {
            return EqualityComparer<NavigationRequest>.Default.Equals(left!, right!);
        }

        /// <inheritdoc/>
        public static bool operator !=(NavigationRequest? left, NavigationRequest? right)
        {
            return !(left == right);
        }

        #endregion
    }

    /// <summary>
    /// Extension methods for <see cref="NavigationRequest"/>.
    /// </summary>
    public static class NavigationRequestEx
    {
        /// <summary>
        /// Append a value to the path.
        /// </summary>
        /// <param name="request">The navigation request.</param>
        /// <param name="value">The value to append.</param>
        /// <returns>This object for further configuration.</returns>
        public static NavigationRequest AddPath(this NavigationRequest request, string value)
        {
            request.Paths.Add(value);
            return request;
        }

        /// <summary>
        /// Add multiple paths at once.
        /// </summary>
        /// <param name="request">The navigation request.</param>
        /// <param name="paths">The paths to add.</param>
        /// <returns>This object for further configuration.</returns>
        public static NavigationRequest AddPaths(this NavigationRequest request, IEnumerable<string> paths)
        {
            return request.AddPaths(paths.ToArray());
        }

        /// <summary>
        /// Add multiple paths at once.
        /// </summary>
        /// <param name="request">The navigation request.</param>
        /// <param name="paths">The paths to add.</param>
        /// <returns>This object for further configuration.</returns>
        public static NavigationRequest AddPaths(this NavigationRequest request, params string[] paths)
        {
            var count = paths.Length;
            string path;
            for (int i = 0; i < count; i++)
            {
                path = paths[i];
                request.Paths.Add(path);
            }
            return request;
        }

        /// <summary>
        /// Append a value to the query.
        /// </summary>
        /// <param name="request">The navigation request.</param>
        /// <param name="key">The query parameter key, must be unique.</param>
        /// <param name="value">The query parameter value. This is not escaped by default.</param>
        /// <returns>This object for further configuration.</returns>
        public static NavigationRequest AddQuery(this NavigationRequest request, string key, string value)
        {
            request.Queries.Add(key, value);
            return request;
        }

        /// <summary>
        /// Append an object to the query. The object's <see cref="object.ToString()"/> will be executed.
        /// </summary>
        /// <param name="request">The navigation request.</param>
        /// <param name="key">The query parameter key, must be unique.</param>
        /// <param name="value">The query parameter value. This is not escaped by default.</param>
        /// <returns>This object for further configuration.</returns>
        public static NavigationRequest AddQuery(this NavigationRequest request, string key, object value)
        {
            return request.AddQuery(key, value.ToString()!);
        }

        /// <summary>
        /// Get a path at the provided index.
        /// </summary>
        /// <param name="request">The request to get the path from.</param>
        /// <param name="index">The index of the path to retrieve.</param>
        /// <returns>The path at the provided index.</returns>
        /// <exception cref="IndexOutOfRangeException">When the index doesn't exist.</exception>
        public static string GetPath(this NavigationRequest request, int index)
        {
            return request.Paths[index];
        }

        /// <summary>
        /// Get a path at the provided index or <see langword="null"/> if it doesn't exist.
        /// </summary>
        /// <param name="request">The request to get the path from.</param>
        /// <param name="index">The index of the path to retrieve.</param>
        /// <returns>The path at the provided index or null.</returns>
        public static string? GetPathOrDefault(this NavigationRequest request, int index)
        {
            return request.Paths.ElementAtOrDefault(index);
        }

        /// <summary>
        /// Get a path at the provided index or <paramref name="defaultValue"/> if it doesn't exist.
        /// </summary>
        /// <param name="request">The request to get the path from.</param>
        /// <param name="index">The index of the path to retrieve.</param>
        /// <param name="defaultValue">A value to return if the index doesnt exist.</param>
        /// <returns>The value at index or the default value.</returns>
        public static string GetPathOrDefault(this NavigationRequest request, int index, string defaultValue)
        {
            return request.Paths.ElementAtOrDefault(index) ?? defaultValue;
        }

        /// <summary>
        /// Get a query at for the provided key.
        /// </summary>
        /// <param name="request">The request to get the query value from.</param>
        /// <param name="key">The key of the query value to retrieve.</param>
        /// <returns>The query value for the provided key.</returns>
        /// <exception cref="KeyNotFoundException">When the key doesn't exist.</exception>
        public static string GetQuery(this NavigationRequest request, string key)
        {
            return request.Queries[key];
        }

        /// <summary>
        /// Get a query for the provided key or <see langword="null"/> if it doesn't exist.
        /// </summary>
        /// <param name="request">The request to get the query value from.</param>
        /// <param name="key">The key of the query value to retrieve.</param>
        /// <returns>The query value for the provided key or null.</returns>
        public static string? GetQueryOrDefault(this NavigationRequest request, string key)
        {
            return request.Queries.TryGetValue(key, out var query) ? query : null;
        }

        /// <summary>
        /// Get a query for the provided key or <paramref name="defaultValue"/> if it doesn't exist.
        /// </summary>
        /// <param name="request">The request to get the query value from.</param>
        /// <param name="key">The key of the query value to retrieve.</param>
        /// <param name="defaultValue">A value to return if the key doesnt exist.</param>
        /// <returns>The query value for the provided <paramref name="key"/> or <paramref name="defaultValue"/>.</returns>
        public static string GetQueryOrDefault(this NavigationRequest request, string key, string defaultValue)
        {
            return request.Queries.TryGetValue(key, out var query) ? query : defaultValue;
        }
    }
}
