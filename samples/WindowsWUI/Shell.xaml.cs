using Base;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsWUI
{
    public abstract class ShellBase : WindowBase<ShellViewModel> { }

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class Shell : ShellBase
    {
        public Shell()
        {
            InitializeComponent();
            this.ResolveViewModel();

            //this.BindCommand(ViewModel, vm => vm.Navigate, v => v.page1, Observable.Return("page1"));
            //this.BindCommand(ViewModel, vm => vm.Navigate, v => v.page2, Observable.Return("page2"));
            //this.BindCommand(ViewModel, vm => vm.Navigate, v => v.page3, Observable.Return("page3"));

        }
    }
}
