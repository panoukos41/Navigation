using Base;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowsWUI
{
    public class ShellBase : ReactivePage<ShellViewModel> { }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : ShellBase
    {
        public Shell()
        {
            InitializeComponent();
            this.ResolveViewModel();

            this.WhenActivated((CompositeDisposable d) =>
            {
                this.BindCommand(ViewModel, vm => vm.Navigate, v => v.page1, Observable.Return("page1"));
                this.BindCommand(ViewModel, vm => vm.Navigate, v => v.page2, Observable.Return("page2"));
                this.BindCommand(ViewModel, vm => vm.Navigate, v => v.page3, Observable.Return("page3"));
            });
        }
    }
}
