using P41.Navigation;
using System.Linq;

namespace UI.Windows;

public sealed partial class Shell : Window
{
    public ShellViewModel? ViewModel { get; set; }

    private bool ignoreMenuEvents = false;

    public Shell(NavigationHost host)
    {
        InitializeComponent();
        ViewModel = Services.Resolve<ShellViewModel>();
        host.Host = MainFrame;

        host.WhenNavigated(host => View.IsBackEnabled = host.Count > 1);

        View.Events().BackRequested.Subscribe(_ =>
        {
            ignoreMenuEvents = true;
            host.GoBack();
            View.SelectedItem = View.MenuItems.OfType<NavigationViewItem>().First(item => host.CurrentRequest!.ToString().Contains((string)item.Tag));
            ignoreMenuEvents = false;
        });

        View.Events().SelectionChanged
            .Where(_ => ignoreMenuEvents is false)
            .Select(x => (string)((NavigationViewItem)x.args.SelectedItem).Tag)
            .InvokeCommand(ViewModel!.Navigate);

        ignoreMenuEvents = true;
        View.SelectedItem = View.MenuItems[0];
        host.Navigate("page1");
        ignoreMenuEvents = false;
    }
}
