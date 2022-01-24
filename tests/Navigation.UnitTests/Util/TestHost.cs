using Flurl;
using P41.Navigation.Host;
using System.Collections.Generic;

namespace P41.Navigation.UnitTests.Util;

class TestHost : NavigationHostBase<TestView, TestHost>
{
    public Stack<Url> PushHistory { get; } = new();

    public Stack<Url> PopHistory { get; } = new();

    public Stack<object> Views { get; } = new();

    protected override object PlatformNavigate(TestView view)
    {
        PushHistory.Push(CurrentRequest ?? "");

        Views.Push(view);
        return view;
    }

    protected override object? PlatformGoBack()
    {
        PopHistory.Push(CurrentRequest ?? "");

        Views.Pop();
        var view = Views.Peek();

        return view;
    }
}
