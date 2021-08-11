using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;

namespace P41.Navigation.UnitTests.Util
{
    [DebuggerDisplay("NavigatedTo: {NavigatedToCount}, NavigatedFrom: {NavigatingFromCount}")]
    class TestViewModel : ReactiveObject, INavigationAware
    {
        public int NavigatedToCount { get; private set; }

        public int NavigatingFromCount { get; private set; }

        public IObservable<Unit> NavigatedTo(NavigationRequest parameters)
        {
            NavigatedToCount++;
            return Observable.Return(Unit.Default);
        }

        public IObservable<Unit> NavigatingFrom()
        {
            NavigatingFromCount++;
            return Observable.Return(Unit.Default);
        }
    }
}
