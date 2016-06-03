using ReactNative;
using Salesforce.SDK.App;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Core;
using Salesforce.SDK.Logging;
using Salesforce.SDK.Security;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Salesforce.SalesforceReactApp
{
    sealed internal class Logger : ILoggingService
    {
        public void Log(Exception exception, LoggingLevel loggingLevel, [CallerMemberName] string memberName = "", [CallerFilePath] string classFilePath = "", [CallerLineNumber]int line = 0)
        {
            if (exception != null)
            {
                Log(exception.Message, loggingLevel, memberName, classFilePath, line);
            }
        }

        public void Log(string message, LoggingLevel loggingLevel, [CallerMemberName] string memberName = "", [CallerFilePath] string classFilePath = "", [CallerLineNumber]int line = 0)
        {
            Debug.WriteLine($"{loggingLevel}:{Path.GetFileName(classFilePath)}:{memberName}:line {line} - {message}");
        }
    }

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : SalesforceApplication
    {
        private readonly ReactPage _reactPage;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Resuming += OnResuming;

            _reactPage = new MainPage();
        }

        public ReactPage ReactNativePage
        {
            get
            {
                return _reactPage;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            _reactPage.OnResume(Exit);

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                _reactPage.OnCreate();
            }

            base.OnLaunched(e);

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Visible;
#endif
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
        ///     InitializeConfig should implement the commented out code. You should come up with your own, unique password and
        ///     salt and for added security
        ///     you should implement your own key generator using the IKeyGenerator interface.
        /// </summary>
        /// <returns></returns>
        protected override Task InitializeConfig()
        {
            SDKServiceLocator.RegisterService<IEncryptionService, Encryptor>();
            SDKServiceLocator.RegisterService<ILoggingService, Logger>();
            Encryptor.init(new EncryptionSettings(new HmacSHA256KeyGenerator()));
            var config = SDKManager.InitializeConfigAsync<Config>().Result;
            return config.SaveConfigAsync();
        }

        /// <summary>
        ///     This returns the root application of your application. Please adjust to match your actual root page if you use
        ///     something different.
        /// </summary>
        /// <returns></returns>
        protected override Type SetRootApplicationPage()
        {
            return typeof(LoginPage);
        }

        /// <summary>
        /// Invoked when application execution is being resumed.
        /// </summary>
        /// <param name="sender">The source of the resume request.</param>
        /// <param name="e">Details about the resume request.</param>
        private void OnResuming(object sender, object e)
        {            
            _reactPage.OnResume(Exit);
        }
    }
}
