using Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using P41.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsWUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (Window.Current is { })
            {
                Window.Current.Activate();
                return;
            }

            Services.Initialize(services =>
            {
                services.AddSingleton(sp =>
                    new NavigationHost()
                    .AddPair("page1", static () => typeof(Page1), static () => Services.Resolve<Page1ViewModel>())
                    .AddPair("page2", static () => typeof(Page2), static () => Services.Resolve<Page2ViewModel>())
                    .AddPair("page3", static () => typeof(Page3), static () => Services.Resolve<Page3ViewModel>())
                );

                services.AddSingleton<INavigationHost>(sp => sp.GetRequiredService<NavigationHost>());
            });

            //shell ??= new Shell();
            //Services.Resolve<NavigationHost>().Host = shell.MainFrame;

            //shell.Activate();

            var main = new MainWindow();
            main.Activate();
        }

        private Shell? shell;
    }
}
