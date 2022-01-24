using P41.Navigation;
using msui = Microsoft.UI.Xaml.Controls;

namespace UI.Uwp;

public sealed partial class Shell : Windows.UI.Xaml.Controls.Page
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
            View.SelectedItem = View.MenuItems.OfType<msui.NavigationViewItem>().First(item => host.CurrentRequest!.ToString().Contains((string)item.Tag));
            ignoreMenuEvents = false;
        });

        View.Events().SelectionChanged
            .Where(_ => ignoreMenuEvents is false)
            .Select(x => (string)((msui.NavigationViewItem)x.args.SelectedItem).Tag)
            .InvokeCommand(ViewModel!.Navigate);

        ignoreMenuEvents = true;
        View.SelectedItem = View.MenuItems[0];
        host.Navigate("page1");
        ignoreMenuEvents = false;
    }
}
