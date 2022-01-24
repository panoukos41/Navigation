using ReactiveUI;
using System.Diagnostics;
using System.Reactive.Disposables;

namespace P41.Navigation.UnitTests.Util;

[DebuggerDisplay("NavigatedTo: {NavigatedToCount}, NavigatedFrom: {NavigatingFromCount}")]
class TestViewModel : ReactiveObject, INavigatableViewModel, IActivatableViewModel
{
    public int NavigatedToCount { get; private set; }

    public int NavigatingFromCount { get; private set; }

    public ViewModelNavigator Navigator { get; } = new();

    public ViewModelActivator Activator { get; } = new();

    public TestViewModel()
    {
        this.WhenActivated((CompositeDisposable d) =>
        {
            d.Add(Disposable.Create(() =>
            {

            }));
        });

        this.WhenNavigatedTo((url, d) =>
        {
            NavigatedToCount++;

            d.Add(Disposable.Create(() =>
            {
                NavigatingFromCount++;
            }));
        });
    }
}
