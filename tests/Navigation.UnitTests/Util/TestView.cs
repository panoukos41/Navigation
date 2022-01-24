using ReactiveUI;
using System.Diagnostics;

namespace P41.Navigation.UnitTests.Util;

[DebuggerDisplay("Page: {page}")]
class TestView : IViewFor<TestViewModel>
{
    private readonly string page;

    public TestView(string page)
    {
        this.page = page;
        ViewModel = new();
    }

    public TestViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TestViewModel?)value;
    }
}
