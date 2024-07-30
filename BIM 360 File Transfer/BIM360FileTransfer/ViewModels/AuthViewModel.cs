using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using CefSharp.Wpf;
using CefSharp;
using Autodesk.Forge;
using BIM360FileTransfer.VIews;
using BIM360FileTransfer.Models;
using BIM360FileTransfer.Commands;
using BIM360FileTransfer.Interfaces;

namespace BIM360FileTransfer.ViewModels
{
    internal class AuthViewModel : BaseViewModel, IViewModel
    {
        #region Data
        public ChromiumWebBrowser authBrowser;
        public OAuthWindow oAuthWindow;
        public FileBrowseViewModel fileBrowseViewModel;
        #endregion


        #region Constructor
        public AuthViewModel(FileBrowseViewModel fileBrowseViewModel)
        {
            this.fileBrowseViewModel = fileBrowseViewModel;
            OpenAuthCommand = new AuthCommand(this);
        }
        #endregion


        #region Get and refresh token
        /// <summary>
        /// Open an authentication window using Chromium browser.
        /// </summary>
        public void OpenAuthPage()
        {
            // Set up the Chromium cache. 
            var settings = new CefSettings();
            settings.CachePath = AppDomain.CurrentDomain.BaseDirectory + "cache";
            if (!Cef.IsInitialized) Cef.Initialize(settings);

            // Open the authentication window.
            oAuthWindow = new OAuthWindow();
            oAuthWindow.Show();
            authBrowser = oAuthWindow.webBrowser;
            authBrowser.MinHeight = 600;
            authBrowser.MinWidth = 300;

            // Build authentication url.
            string authorizeURL = User.FORGE_BASE_URL +
                string.Format("/authentication/v1/authorize?response_type=code&client_id={0}&redirect_uri={1}&scope={2}",
                User.FORGE_CLIENT_ID, User.FORGE_CALLBACK_URL, System.Net.WebUtility.UrlEncode(User.FORGE_SCOPE));

            // Open url and get end event.
            authBrowser.LoadUrl(authorizeURL);
            authBrowser.FrameLoadEnd += BrowserFrameLoadEnd;


        }

        /// <summary>
        /// Get authentication code from the call back url. Close browser window once done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private async void BrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            // Find the final call back url with code.
            if (!e.Url.Contains(User.Forge_Code_Pattern))
            {
                return;
            }
            else
            {
                // Close the authentication window once done.
                if (oAuthWindow.Dispatcher.CheckAccess())
                    oAuthWindow.Close();
                else
                    oAuthWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(oAuthWindow.Close));

                // Remove the load end event.
                authBrowser.FrameLoadEnd -= BrowserFrameLoadEnd;

                // Get the token.
                User.FORGE_CODE = e.Url.Substring(e.Url.IndexOf(User.Forge_Code_Pattern) + User.Forge_Code_Pattern.Length);
                if (User.FORGE_CODE is null)
                {
                    throw new ArgumentNullException("Unable to get the authentication code. Please check your client_id and client_secret.", nameof(User.FORGE_CODE));
                }
                User.FORGE_INTERNAL_TOKEN = await Get3LeggedTokenAsync();
                fileBrowseViewModel.GetCategoryAsync();

                var dueTime = TimeSpan.FromSeconds(10);
                var interval = 

                // TODO: Add a CancellationTokenSource and supply the token here instead of None.
                _ = RunPeriodicAsync(OnTick, dueTime, CancellationToken.None);
            }
        }

        /// <summary>
        /// Get 3 legged token from Forge API.
        /// </summary>
        /// <returns>Bearer token</returns>
        private async Task<dynamic> Get3LeggedTokenAsync()
        {
            ThreeLeggedApi oauth = new ThreeLeggedApi();
            dynamic bearer = await oauth.GettokenAsync(
              User.FORGE_CLIENT_ID,
              User.FORGE_CLIENT_SECRET,
              User.FORGE_GRANT_TYPE,
              User.FORGE_CODE,
              User.FORGE_CALLBACK_URL);
            return bearer;
        }

        /// <summary>
        /// Refresh the 3 legged token from Forge API.
        /// </summary>
        /// <returns></returns>
        private async Task<dynamic> RefreshTokenAsync()
        {
            ThreeLeggedApi oauth = new ThreeLeggedApi();
            dynamic bearer = await oauth.RefreshtokenAsync(
              User.FORGE_CLIENT_ID,
              User.FORGE_CLIENT_SECRET,
              "refresh_token",
              User.FORGE_INTERNAL_TOKEN.refresh_token);
            return bearer;
        }
        #endregion


        #region Run periodic function
        private async void OnTick()
        {
            User.FORGE_INTERNAL_TOKEN = await RefreshTokenAsync();
        }

        private static async Task RunPeriodicAsync(Action onTick,
                                           TimeSpan dueTime,
                                           CancellationToken token)
        {
            var interval = TimeSpan.FromSeconds(User.FORGE_INTERNAL_TOKEN.expires_in - 60);

            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }
        #endregion


        #region ICommand
        public bool CanOpenAuthPage
        {
            get
            {
                return true;
            }
        }

        public ICommand OpenAuthCommand
        {
            get;
            private set;
        }

        public ICommand FileBrowseCommand
        {
            get;
            private set;
        }
        #endregion
    }
}
