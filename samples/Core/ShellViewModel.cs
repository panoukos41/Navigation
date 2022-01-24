using Flurl;
using P41.Navigation;
using ReactiveUI;
using System.Reactive;

namespace Core;

public class ShellViewModel : ViewModel
{
    public ReactiveCommand<string, Unit> Navigate { get; }

    public ReactiveCommand<Unit, Unit> GoBack { get; }

    public ShellViewModel(INavigationHost host)
    {
        Navigate = ReactiveCommand.Create<string>(page =>
        {
            host.Navigate(page);
        });

        GoBack = ReactiveCommand.Create<Unit>(_ => host.GoBack());
    }
}
