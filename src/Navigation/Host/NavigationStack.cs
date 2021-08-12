using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace P41.Navigation.Host
{
    /// <summary>
    /// A class that inherits <see cref="Stack{T}"/> where T is <see cref="NavigationRequest"/>
    /// and implements <see cref="IObservable{T}"/> events for the stack operations.
    /// </summary>
    public class NavigationStack : Stack<NavigationRequest>, IEquatable<NavigationStack>, IEquatable<Stack<NavigationRequest>>
    {
        private readonly Subject<NavigationRequest> _change = new();
        private readonly Subject<NavigationRequest> _pushed = new();
        private readonly Subject<NavigationRequest> _popped = new();

        /// <summary>
        /// Initialize a new instance of <see cref="NavigationStack"/> that
        /// is empty and has the default initial capacity.
        /// </summary>
        public NavigationStack() : base()
        {
        }

        /// <summary>
        /// Initialzie a new instance of <see cref="NavigationStack"/> that
        /// contains elements copied from the specified requests collection
        /// and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="requests">The collection to copy requests from.</param>
        public NavigationStack(IEnumerable<NavigationRequest> requests) : base(requests)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationStack"/> that
        /// is empty and has the specified initial capacity or the default initial capacity,
        /// whichever is greater.
        /// </summary>
        /// <param name="capacity">The initial number of requests that the <see cref="NavigationStack"/>
        /// can contain.</param>
        public NavigationStack(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// The number of elements in the stack.
        /// </summary>
        public new int Count => base.Count;

        /// <summary>
        /// An observable that signals a change and
        /// passes the top element <see cref="Peek"/>.
        /// </summary>
        public IObservable<NavigationRequest> Change =>
            _change.AsObservable();

        /// <summary>
        /// An observable that signals objects are pushed to the stack.
        /// </summary>
        public IObservable<NavigationRequest> Pushed =>
            _pushed.AsObservable();

        /// <summary>
        /// An observable that signals objects are popped from the stack.
        /// the navigation stack.
        /// </summary>
        public IObservable<NavigationRequest> Popped =>
            _popped.AsObservable();

        /// <summary>
        /// Push a new page/parameters pair to the stack.
        /// </summary>
        /// <param name="request"></param>
        public new Unit Push(NavigationRequest request)
        {
            base.Push(request);
            _pushed.OnNext(request);
            _change.OnNext(request);

            return Unit.Default;
        }

        /// <summary>
        /// Pop the current page/parameters pair from the stack and
        /// return it.
        /// </summary>
        /// <returns>The item that was removed from the stack.</returns>
        public new NavigationRequest Pop()
        {
            var obj = base.Pop();
            _popped.OnNext(obj);
            _change.OnNext(Peek());

            return obj;
        }

        /// <summary>
        /// Return the page/parameters pair at the top of the stack.
        /// </summary>
        /// <returns></returns>
        public new NavigationRequest Peek() => base.Peek();

        /// <summary>
        /// Returns the output of <see cref="ToJson(bool)"/>
        /// passing <see langword="true"/> as the argument.
        /// </summary>
        /// <returns>A pretty json representation.</returns>
        public override string ToString()
        {
            return ToJson(true);
        }

        /// <summary>
        /// Turn this <see cref="NavigationStack"/> to a json array
        /// of <see cref="NavigationRequest"/> strings.
        /// </summary>
        /// <param name="indented">If json should be pretty printed or not.</param>
        /// <returns>A json representation of the stack.</returns>
        public string ToJson(bool indented = false)
        {
            var sb = new StringBuilder();

            sb.Append('[');
            AppendLine();

            var items = ToArray();
            var last = items.Length - 1;
            for (int i = 0; i < last; i++)
            {
                AppendRequest(items[i]);
                sb.Append(',');
                AppendLine();
            }

            AppendRequest(items[last]);
            AppendLine();

            sb.Append(']');

            return sb.ToString();

            void AppendLine()
            {
                if (indented) sb.Append('\n');
            }

            void AppendRequest(NavigationRequest request)
            {
                if (indented) sb.Append(' ', 2);
                sb.Append('"');
                sb.Append(request.ToString());
                sb.Append('"');
            }
        }

        /// <summary>
        /// Deserialize a string to <see cref="NavigationStack"/>.
        /// </summary>
        /// <param name="json">The json to deserialize from.</param>
        /// <returns></returns>
        public static NavigationStack Parse(string json)
        {
            var stack = new NavigationStack();

            json = json.Replace("\n", "").Replace("  ", "");
            json = json.Remove(0, 1);
            json = json.Remove(json.Length - 1);

            foreach (var item in json.Split(',').Reverse())
            {
                ((Stack<NavigationRequest>)stack).Push(NavigationRequest.Parse(item.Trim('"')));
            }

            return stack;
        }

        #region Equals

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is NavigationStack stack
                && Equals(stack);
        }

        /// <inheritdoc/>
        public bool Equals(NavigationStack? other)
        {
            return other is Stack<NavigationRequest> stack
                && Equals(stack);
        }

        /// <inheritdoc/>
        public bool Equals(Stack<NavigationRequest>? other)
        {
            if (other is null) return false;

            return other.SequenceEqual(this);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this
                .Select(r => r.GetHashCode())
                .Aggregate((l, r) => HashCode.Combine(l, r));
        }

        /// <inheritdoc/>
        public static bool operator ==(NavigationStack? left, NavigationStack? right)
        {
            return EqualityComparer<NavigationStack>.Default.Equals(left!, right!);
        }

        /// <inheritdoc/>
        public static bool operator !=(NavigationStack? left, NavigationStack? right)
        {
            return !(left == right);
        }

        #endregion
    }
}