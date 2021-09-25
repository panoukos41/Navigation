using Flurl;
using P41.Navigation;
using ReactiveUI;
using System.Reactive;

namespace Base
{
    public class ShellViewModel : ReactiveObject
    {
        public ReactiveCommand<string, Unit> Navigate { get; }

        public ReactiveCommand<Unit, Unit> GoBack { get; }

        public ShellViewModel(INavigationHost host)
        {
            Navigate = ReactiveCommand.Create<string>(page =>
            {
                var request = new Url(page);

                request.AppendPathSegment(page switch
                {
                    "page1" => "1",
                    "page2" => "2",
                    "page3" => "3",
                    _ => "none"
                });

                host.Navigate(request);
            });

            GoBack = ReactiveCommand.Create<Unit>(_ => host.GoBack());
        }
    }
}
