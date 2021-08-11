using P41.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
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

        public IObservable<Unit> NavigatedTo(NavigationRequest parameters)
        {
            if (int.TryParse(parameters.GetPathOrDefault(0, "10"), out var inc))
            {
                IncreaseBy *= inc;
            }
            return Observable.Return(Unit.Default);
        }

        public IObservable<Unit> NavigatingFrom()
        {
            return Observable.Return(Unit.Default);
        }
    }
}
