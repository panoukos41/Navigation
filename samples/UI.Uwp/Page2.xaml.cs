namespace UI.Uwp;

public abstract class Page2Base : ReactivePage<Page2ViewModel> { }

public sealed partial class Page2 : Page2Base
{
    public Page2()
    {
        this.InitializeComponent();
        this.ResolveViewModel();

        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.Text, v => v.TextBlock.Text);
        });
    }
}
