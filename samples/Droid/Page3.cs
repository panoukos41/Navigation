namespace Droid;

public class Page3 : ReactiveFragment<Page3ViewModel>
{
    public override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = inflater.Inflate(R.Layout.fragment_page3, container, false);

        return view!;
    }
}
