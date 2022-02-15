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
using CefSharp.Wpf;
using CefSharp;
using Autodesk.Forge;
using BIM360FileTransfer.VIews;
using BIM360FileTransfer.Models;
using BIM360FileTransfer.Commands;
using BIM360FileTransfer.Interfaces;

namespace BIM360FileTransfer.ViewModels
{
    internal class AuthViewModel
    {
        private const string FORGE_CLIENT_ID = "Oc1Dgsd4bxY5hbfvYOsuHCkZTyI1ef7q";
        private const string FORGE_CLIENT_SECRET = "PwA4iw3Ant6MlkQZ";
        private const string FORGE_CALLBACK_URL = "http://sampleapp.com/oauth/callback?foo=bar";
        private const string FORGE_BASE_URL = "https://developer.api.autodesk.com";
        private const string FORGE_SCOPE = "data:read data:write data:create data:search bucket:create bucket:read bucket:update bucket:delete"; // Full scope access.
        private const string FORGE_GRANT_TYPE = "authorization_code";
        private const string Forge_Code_Pattern = "&code=";
        private static string FORGE_CODE { get; set; }
        private static dynamic FORGE_INTERNAL_TOKEN { get; set; }


        public ChromiumWebBrowser authBrowser;
        public OAuthWindow oAuthWindow;



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
            string authorizeURL = FORGE_BASE_URL +
                string.Format("/authentication/v1/authorize?response_type=code&client_id={0}&redirect_uri={1}&scope={2}",
                FORGE_CLIENT_ID, FORGE_CALLBACK_URL, System.Net.WebUtility.UrlEncode(FORGE_SCOPE));

            // Open url and get end event.
            authBrowser.LoadUrl(authorizeURL);
            authBrowser.FrameLoadEnd += BrowserFrameLoadEnd;
        }

        private async void BrowserFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            // Find the final call back url with code.
            if (!e.Url.Contains(Forge_Code_Pattern))
            {
                return;
            }
            else
            {
                // Remove the load end event.
                authBrowser.FrameLoadEnd -= BrowserFrameLoadEnd;

                // Get the token.
                FORGE_CODE = e.Url.Substring(e.Url.IndexOf(Forge_Code_Pattern) + Forge_Code_Pattern.Length);
                if (FORGE_CODE is null)
                {
                    throw new ArgumentNullException("Unable to get the authentication code. Please check your client_id and client_secret.", nameof(FORGE_CODE));
                }
                FORGE_INTERNAL_TOKEN = await Get3LeggedTokenAsync();


                // Close the authentication window once done.
                if (oAuthWindow.Dispatcher.CheckAccess())
                    oAuthWindow.Close();
                else
                    oAuthWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(oAuthWindow.Close));
            }
        }

        private async Task<dynamic> Get3LeggedTokenAsync()
        {
            ThreeLeggedApi oauth = new ThreeLeggedApi();
            dynamic bearer = await oauth.GettokenAsync(
              FORGE_CLIENT_ID,
              FORGE_CLIENT_SECRET,
              FORGE_GRANT_TYPE,
              FORGE_CODE,
              FORGE_CALLBACK_URL);
            return bearer;
        }
    }
}
