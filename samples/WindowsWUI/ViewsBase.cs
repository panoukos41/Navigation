using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ReactiveUI;

namespace WindowsWUI
{
    public abstract class PageBase<T> : Page, IViewFor<T>
        where T : class
    {
        public T? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (T?)value;
        }
    }

    public abstract class WindowBase<T> : Window, IViewFor<T>
        where T : class
    {
        public T? ViewModel { get; set; }

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (T?)value;
        }
    }
}
