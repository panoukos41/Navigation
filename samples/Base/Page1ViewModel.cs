using Flurl;
using P41.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Base
{
    public class Page1ViewModel : ReactiveObject, INavigationAware
    {
        [Reactive]
        public int Count { get; private set; }

        [Reactive]
        public int IncreaseBy { get; private set; } = 1;

        public ReactiveCommand<Unit, Unit> Increase { get; }

        public Page1ViewModel()
        {
            Increase = ReactiveCommand.Create(() =>
            {
                Count += IncreaseBy;
            });
        }

        public IObservable<Unit> NavigatedTo(Url request, INavigationHost host)
        {
            IncreaseBy = int.TryParse(request.PathSegments.LastOrDefault(), out var inc)
                ? inc : 10;

            return Observable.Return(Unit.Default);
        }

        public IObservable<Unit> NavigatingFrom()
        {
            return Observable.Return(Unit.Default);
        }
    }
}
