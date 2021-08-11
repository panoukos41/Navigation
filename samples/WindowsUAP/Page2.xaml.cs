using Base;
using ReactiveUI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsWUI
{
    public class Page2Base : ReactivePage<Page2ViewModel> { }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page2 : Page2Base
    {
        public Page2()
        {
            this.InitializeComponent();
        }
    }
}
