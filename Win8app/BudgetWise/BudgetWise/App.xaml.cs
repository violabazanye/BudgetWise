using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Microsoft.Live;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace BudgetWise
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

        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://w8budgetwise.azure-mobile.net/", "ZFWarBBouFiDGSZrMtJCAGaZfqFViC99");

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                BudgetWise.Common.SuspensionManager.RegisterFrame(rootFrame, "appFrame");

                //set the backgorund image
                rootFrame.Background = new ImageBrush
                {
                    Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill,
                    ImageSource = new BitmapImage { UriSource = new Uri("ms-appx:///Images/background-wallpaper-hd-white.jpg")} 
                };

                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                    await BudgetWise.Common.SuspensionManager.RestoreAsync();
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
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
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            await BudgetWise.Common.SuspensionManager.SaveAsync();
            deferral.Complete();
        }


        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
        }

        void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs e)
        {
            SettingsCommand privacyCommand = new SettingsCommand("privacy", "Privacy", (handler) =>
            {
                Privacy privacyFlyout = new Privacy();
                privacyFlyout.Show();
            });
            e.Request.ApplicationCommands.Add(privacyCommand);

            SettingsCommand accountCommand = new SettingsCommand("account", "Account", (handler) =>
            {
                Account accountFlyout = new Account();
                accountFlyout.Show();
            });
            e.Request.ApplicationCommands.Add(accountCommand);

            SettingsCommand aboutCommand = new SettingsCommand("about", "About", (handler) => 
            {
                About aboutFlyout = new About();
                aboutFlyout.Show();
            });
            e.Request.ApplicationCommands.Add(aboutCommand);
         }

        public static async Task updateUserName(TextBlock userName, Boolean signIn)
        {
            try
            {
                // Open Live Connect SDK client.
                LiveAuthClient LCAuth = new LiveAuthClient();
                LiveLoginResult LCLoginResult = await LCAuth.InitializeAsync();
                try
                {
                    LiveLoginResult loginResult = null;
                    if (signIn)
                    {
                        // Sign in to the user's Microsoft account with the required scope.
                        //  
                        //  This call will display the Microsoft account sign-in screen if 
                        //   the user is not already signed in to their Microsoft account 
                        //   through Windows 8.
                        // 
                        //  This call will also display the consent dialog, if the user has 
                        //   has not already given consent to this app to access the data 
                        //   described by the scope.
                        // 
                        //  Change the parameter of LoginAsync to include the scopes 
                        //   required by your app.
                        loginResult = await LCAuth.LoginAsync(new string[] { "wl.basic" });
                    }
                    else
                    {
                        // If we don't want the user to sign in, continue with the current 
                        //  sign-in state.
                        loginResult = LCLoginResult;
                    }
                    if (loginResult.Status == LiveConnectSessionStatus.Connected)
                    {
                        // Create a client session to get the profile data.
                        LiveConnectClient connect = new LiveConnectClient(LCAuth.Session);

                        // Get the profile info of the user.
                        LiveOperationResult operationResult = await connect.GetAsync("me");
                        dynamic result = operationResult.Result;
                        if (result != null)
                        {
                            // Update the text of the object passed in to the method. 
                            userName.Text = string.Join(" ", "Hello", result.name, "!");
                        }
                        else
                        {
                            // Handle the case where the user name was not returned. 
                        }
                    }
                    else
                    {
                        // The user hasn't signed in so display this text 
                        //  in place of his or her name.
                        userName.Text = "You're not signed in.";
                    }
                }
                catch (LiveAuthException exception)
                {
                    // Handle the exception.
                    userName.Text = exception.Message.ToString();
                    throw new NotImplementedException();
                }
            }
            catch (LiveAuthException exception)
            {
                // Handle the exception.
                userName.Text = exception.Message.ToString();
                throw new NotImplementedException();
            }
            catch (LiveConnectException exception)
            {
                // Handle the exception. 
                userName.Text = exception.Message.ToString();
                throw new NotImplementedException();
            }
        }
       
    }
}
