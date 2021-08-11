using Base;
using ReactiveUI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsWUI
{
    public class Page1Base : ReactivePage<Page1ViewModel> { }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page1 : Page1Base
    {
        public Page1()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Count, v => v.Counter.Text, vm => vm.ToString());
                this.Bind(ViewModel, vm => vm.IncreaseBy, v => v.IncreaseBy.Text, vm => vm.ToString(), v => int.TryParse(v, out int value) ? value : 1);
                this.BindCommand(ViewModel, vm => vm.Increase, v => v.Increase);
            });
        }
    }
}
