using Base;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsWUI
{
    public class Page1Base : PageBase<Page1ViewModel> { }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page1 : Page1Base
    {
        public Page1()
        {
            InitializeComponent();
        }
    }
}
