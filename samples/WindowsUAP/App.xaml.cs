using Base;
using Flurl;
using Microsoft.Extensions.DependencyInjection;
using P41.Navigation;
using System;
using System.Reactive.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace WindowsWUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var shell = Window.Current.Content as Shell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                Services.Initialize(services =>
                {
                    services.AddSingleton(sp =>
                        new NavigationHost()
                        .Map("page1", static () => Services.Resolve<Page1ViewModel>(), static () => typeof(Page1))
                        .Map("page2", static () => Services.Resolve<Page2ViewModel>(), static () => typeof(Page2))
                        .Map("page3", static () => Services.Resolve<Page3ViewModel>(), static () => typeof(Page3)));

                    services.AddSingleton<INavigationHost>(sp => sp.GetRequiredService<NavigationHost>());
                });

                // Create a Frame to act as the navigation context and navigate to the first page
                shell = new Shell();
                var nav = Services.Resolve<NavigationHost>();
                nav.Host = shell.MainFrame;
                nav.WhenNavigated(static host =>
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                        host.Count > 1 ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
                });

                var manager = SystemNavigationManager.GetForCurrentView();

                Observable.FromEventPattern<BackRequestedEventArgs>(
                    handler => manager.BackRequested += handler,
                    handler => manager.BackRequested -= handler)
                    .Subscribe(static args => Services.Resolve<INavigationHost>().GoBack());

                shell.MainFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {

                }

                // Place the frame in the current Window
                Window.Current.Content = shell;
            }

            if (e.PrelaunchActivated == false)
            {
                if (shell.MainFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    //rootFrame.Navigate(typeof(Shell), e.Arguments);

                }
                // Ensure the current window is active
                Window.Current.Activate();
                Services.Resolve<INavigationHost>().Navigate("page1/100");
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }
    }
}
