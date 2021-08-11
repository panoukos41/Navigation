using Base;
using ReactiveUI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsWUI
{
    public class Page3Base : ReactivePage<Page3ViewModel> { }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page3 : Page3Base
    {
        public Page3()
        {
            this.InitializeComponent();
        }
    }
}
