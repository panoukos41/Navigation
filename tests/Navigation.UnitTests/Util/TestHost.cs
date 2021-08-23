using Flurl;
using P41.Navigation.Host;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace P41.Navigation.UnitTests.Util;

class TestHost : NavigationHostBase
{
    public Stack<string> History { get; } = new();

    public Dictionary<string, IViewFor> Views { get; } = new();

    public Dictionary<string, TestViewModel> ViewModels { get; } = new();

    protected override IObservable<IViewFor> PlatformNavigate()
    {
        var page = request.Page;
        var view = Views.GetValueOrDefault(page);

        if (view is null)
        {
            view = new TestView(page);
            Views.Add(page, view);
        }

        History.Push(page);

        return Observable.Return(view);
    }

    protected override IObservable<IViewFor?> PlatformGoBack()
    {
        History.Pop();

        var page = History.Peek();

        return Observable.Return(Views[page]);
    }

    protected override object? InitializeViewModel()
    {
        var vm = ViewModels.GetValueOrDefault(page);

        if (vm is null)
        {
            vm = new TestViewModel();
            ViewModels.Add(page, vm);
        }

        return vm;
    }
}
