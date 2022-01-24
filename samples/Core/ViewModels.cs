using P41.Navigation;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace Core;

public class ViewModel : ReactiveObject, INavigatableViewModel
{
    public ViewModelNavigator Navigator { get; } = new();
}

public class Page1ViewModel : ViewModel
{
    private double _count;
    private double _increaseBy;

    public double Count { get => _count; private set => this.RaiseAndSetIfChanged(ref _count, value); }

    public double IncreaseBy { get => _increaseBy; private set => this.RaiseAndSetIfChanged(ref _increaseBy, value); }

    public ReactiveCommand<Unit, Unit> Increase { get; }

    public Page1ViewModel()
    {
        Increase = ReactiveCommand.Create(() =>
        {
            Count += IncreaseBy;
        });

        this.WhenNavigatedTo((request, d) =>
        {
            IncreaseBy = int.TryParse(request.PathSegments.LastOrDefault(), out var inc) ? inc : 10;
        });
    }
}

public class Page2ViewModel : ViewModel
{
    string _text = "Page2";

    public string Text { get => _text; set => this.RaiseAndSetIfChanged(ref _text, value); }

    public Page2ViewModel()
    {
        this.WhenNavigatedTo((r, d) =>
        {
            Text = $"Page 2 Request: {r}";
        });
    }
}

public class Page3ViewModel : ViewModel
{

}