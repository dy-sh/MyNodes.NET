using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SerialController_Windows.Code;
using SerialController_Windows.Views;

namespace SerialController_Windows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        public static SerialPort serialPort;
        public static LedStripController ledStripController;
        public static RemoteColorClient remoteColorClient;
        public static SerialController serialController;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            serialPort=new SerialPort();
         //   ledStripController=new LedStripController(serialPort);
            serialController=new SerialController(serialPort);

        }

        public AppShell shell;

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
         //       this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            shell = Window.Current.Content as AppShell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // Create a AppShell to act as the navigation context and navigate to the first page
                shell = new AppShell();

                // Set the default language
                shell.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                shell.AppFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
            }

            Window.Current.Content = shell;


            shell.AppFrame.Navigated += shell.OnPageCanged;


            if (shell.AppFrame.Content == null)
            {
                // When the navigation stack isn't restored, navigate to the first page
                // suppressing the initial entrance animation.
                shell.AppFrame.Navigate(typeof(NodesPage), e.Arguments, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }

            // Ensure the current window is active
            Window.Current.Activate();

            // Ensure the current window is active

            /*
            //COLORIZE TITLE BAR---------------------------------------------------------------------------
           ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = new Color() { A = 255, R = 54, G = 60, B = 116 };
            titleBar.ForegroundColor = new Color() { A = 255, R = 232, G = 211, B = 162 };
            titleBar.InactiveBackgroundColor = new Color() { A = 255, R = 135, G = 141, B = 199 };
            titleBar.InactiveForegroundColor = new Color() { A = 255, R = 232, G = 211, B = 162 };


            titleBar.ButtonBackgroundColor = new Color() { A = 0, R = 54, G = 60, B = 116 };
            titleBar.ButtonHoverBackgroundColor = new Color() { A = 0, R = 19, G = 21, B = 40 };
            titleBar.ButtonPressedBackgroundColor = new Color() { A = 0, R = 232, G = 211, B = 162 };
            titleBar.ButtonInactiveBackgroundColor = new Color() { A = 0, R = 135, G = 141, B = 199 };

            // Title bar button foreground colors. Alpha must be 255.
            titleBar.ButtonForegroundColor = new Color() { A = 255, R = 232, G = 211, B = 162 };
            titleBar.ButtonHoverForegroundColor = new Color() { A = 255, R = 255, G = 255, B = 255 };
            titleBar.ButtonPressedForegroundColor = new Color() { A = 255, R = 54, G = 60, B = 116 };
            titleBar.ButtonInactiveForegroundColor = new Color() { A = 255, R = 232, G = 211, B = 162 };
            ////////////////////////////////////////////////////////////////////////////////////////////////
            */



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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
