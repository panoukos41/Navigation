namespace UI.Uwp;

public abstract class Page1Base : ReactivePage<Page1ViewModel> { }

public sealed partial class Page1 : Page1Base
{
    public Page1()
    {
        this.InitializeComponent();
        this.ResolveViewModel();

        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, vm => vm.Count, v => v.Counter.Text, vm => vm.ToString());
            this.Bind(ViewModel, vm => vm.IncreaseBy, v => v.IncreaseBy.Value);
            this.BindCommand(ViewModel, vm => vm.Increase, v => v.Increase);
        });
    }
}
