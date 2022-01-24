using ReactiveUI;
using System.Reactive.Disposables;

namespace UI.Android.Pages;

public abstract class PageBase<TViewModel> : ReactiveUI.AndroidX.ReactiveFragment<TViewModel>
    where TViewModel : class
{
    private readonly int layoutId;

    protected PageBase(int layoutId)
    {
        this.layoutId = layoutId;
    }

    public override sealed View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = inflater.Inflate(layoutId, container, false);
        this.WireUpControls(view!);
        return view!;
    }
}

public class Page1 : PageBase<Page1ViewModel>
{
    public TextView Count { get; private set; } = null!;
    public EditText IncreaseBy { get; private set; } = null!;
    public Button Increase { get; private set; } = null!;

    public Page1() : base(R.Layout.fragment_page1)
    {
        this.ResolveViewModel();
        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, vm => vm.Count, v => v.Count.Text, vm => vm.ToString()).DisposeWith(d);
            this.Bind(ViewModel, vm => vm.IncreaseBy, v => v.IncreaseBy.Text, vm => vm.ToString(), v => int.TryParse(v, out var n) ? n : 1).DisposeWith(d);
            this.BindCommand(ViewModel, vm => vm.Increase, v => v.Increase).DisposeWith(d);
        });
    }
}

public class Page2 : PageBase<Page2ViewModel>
{
    public TextView TextView { get; private set; } = null!;

    public Page2() : base(R.Layout.fragment_page2)
    {
        this.ResolveViewModel();
        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.Text, v => v.TextView.Text).DisposeWith(d);
        });
    }
}

public class Page3 : PageBase<Page3ViewModel>
{
    public Page3() : base(R.Layout.fragment_page3)
    {
    }
}
