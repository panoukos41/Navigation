namespace Droid;

public class Page2 : ReactiveFragment<Page2ViewModel>
{
    public override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = inflater.Inflate(R.Layout.fragment_page2, container, false);

        return view!;
    }
}
