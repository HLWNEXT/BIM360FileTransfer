using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Threading;
using System.Collections.ObjectModel;
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
        public ChromiumWebBrowser authBrowser;
        public OAuthWindow oAuthWindow;


        public AuthViewModel()
        {
            OpenAuthCommand = new AuthCommand(this);
        }

    public void myFirstCommand(string par)
    {
        Console.Beep();
    }


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
    }
}
