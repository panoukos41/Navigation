using Android.Widget;
using ReactiveUI;

namespace Droid;

public class Page1 : ReactiveUI.AndroidX.ReactiveFragment<Page1ViewModel>
{
    public TextView Count { get; private set; } = null!;
    public EditText IncreaseBy { get; private set; } = null!;
    public Button Increase { get; private set; } = null!;

    // You should probably set up bindings on this.WhenActivated to create them only once!
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = inflater.Inflate(R.Layout.fragment_page1, container, false);

        this.WireUpControls(view!);
        this.OneWayBind(ViewModel, vm => vm.Count, v => v.Count.Text, vm => vm.ToString());
        this.Bind(ViewModel, vm => vm.IncreaseBy, v => v.IncreaseBy.Text, vm => vm.ToString(), v => int.TryParse(v, out var n) ? n : 1);
        this.BindCommand(ViewModel, vm => vm.Increase, v => v.Increase);

        return view!;
    }
}