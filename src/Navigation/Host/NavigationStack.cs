using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace P41.Navigation.Host
{
    /// <summary>
    /// A wrapper around a <see cref="Stack{T}"/> that is used from the <see cref="Navigation"/>.
    /// </summary>
    public class NavigationStack : IReadOnlyCollection<NavigationRequest>
    {
        private readonly Stack<NavigationRequest> _stack;
        private readonly Subject<NavigationRequest> _change = new();
        private readonly Subject<NavigationRequest> _pushed = new();
        private readonly Subject<NavigationRequest> _popped = new();

        /// <summary>
        /// Initialize a new instance of <see cref="NavigationStack"/> that
        /// is empty.
        /// </summary>
        public NavigationStack()
        {
            _stack = new();
        }

        /// <summary>
        /// Initialzie a new instance of <see cref="NavigationStack"/> that
        /// contains elements copied from the specified requests collection.
        /// </summary>
        /// <param name="requests">The collection to copy requests from.</param>
        public NavigationStack(IEnumerable<NavigationRequest> requests)
        {
            _stack = new(requests);
        }

        /// <summary>
        /// The number of elements in the stack.
        /// </summary>
        public int Count => _stack.Count;

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
        /// <param name="stackObject"></param>
        public Unit Push(NavigationRequest stackObject)
        {
            _stack.Push(stackObject);
            _pushed.OnNext(stackObject);
            _change.OnNext(stackObject);

            return Unit.Default;
        }

        /// <summary>
        /// Pop the current page/parameters pair from the stack and
        /// return it.
        /// </summary>
        /// <returns>The item that was removed from the stack.</returns>
        public NavigationRequest Pop()
        {
            var obj = _stack.Pop();
            _popped.OnNext(obj);
            _change.OnNext(Peek());

            return obj;
        }

        /// <summary>
        /// Return the page/parameters pair at the top of the stack.
        /// </summary>
        /// <returns></returns>
        public NavigationRequest Peek() =>
            _stack.Peek();

        #region IEnumerable

        /// <inheritdoc/>
        public IEnumerator<NavigationRequest> GetEnumerator()
        {
            return _stack.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _stack.GetEnumerator();
        }

        #endregion
    }
}