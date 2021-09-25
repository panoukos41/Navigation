using P41.Navigation.Host;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace P41.Navigation.UnitTests.Util;

class TestHost : NavigationHostBase
{
    public Stack<NavigationRoute> History { get; } = new();

    public Dictionary<NavigationRoute, IViewFor> Views { get; }

    public Dictionary<NavigationRoute, TestViewModel> ViewModels { get; }

    public TestHost(Dictionary<NavigationRoute, IViewFor> views, Dictionary<NavigationRoute, TestViewModel> viewModels)
    {
        Views = views;
        ViewModels = viewModels;
    }

    protected override IObservable<IViewFor> PlatformNavigate()
    {
        var route = Views.Keys.First(r => r.Match(CurrentRequest!));
        var view = Views.GetValueOrDefault(route);

        if (view is null)
        {
            view = new TestView(route);
            Views.Add(route, view);
        }
        SetViewModel(view);

        History.Push(route);

        return Observable.Return(view);
    }

    protected override IObservable<IViewFor?> PlatformGoBack()
    {
        History.Pop();

        var page = History.Peek();
        var view = Views[page];
        SetViewModel(view);

        return Observable.Return(view);
    }

    protected override object? InitializeViewModel()
    {
        var route = ViewModels.Keys.First(r => r.Match(CurrentRequest!));
        var vm = ViewModels.GetValueOrDefault(route);

        if (vm is null)
        {
            vm = new TestViewModel();
            ViewModels.Add(route, vm);
        }

        return vm;
    }
}
