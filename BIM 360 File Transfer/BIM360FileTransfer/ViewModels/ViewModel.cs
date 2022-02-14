using BIM360FileTransfer.Models;
using BIM360FileTransfer.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using BIM360FileTransfer.VIews;
using CefSharp.Wpf;
using CefSharp;
using System.Text.RegularExpressions;

namespace BIM360FileTransfer.ViewModels
{
    internal class ViewModel
    {
        private const string FORGE_CLIENT_ID = "Oc1Dgsd4bxY5hbfvYOsuHCkZTyI1ef7q";
        private const string FORGE_CLIENT_SECRET = "PwA4iw3Ant6MlkQZ";
        private const string FORGE_CALLBACK_URL = "http://sampleapp.com/oauth/callback?foo=bar";
        private const string FORGE_BASE_URL = "https://developer.api.autodesk.com";
        private const string FORGE_SCOPE = "data:read data:write data:create data:search bucket:create bucket:read bucket:update bucket:delete"; // Full scope access.




        public ViewModel()
        {
            _AuthModel = new AuthModel("");
            OpenAuthCommand = new AuthCommand(this);
        }


        public bool CanOpenAuthPage
        {
            get
            {
                if (AuthModel is null)
                {
                    return false;
                }
                return AuthModel.Code == "";
            }
        }

        private AuthModel _AuthModel;

        public AuthModel AuthModel
        {
            get { return _AuthModel; }
        }

        public ICommand OpenAuthCommand
        {
            get;
            private set;
        }


        public ChromiumWebBrowser authBrowser;

        public void OpenAuthPage()
        {

            var settings = new CefSettings();
            settings.CachePath = AppDomain.CurrentDomain.BaseDirectory + "cache";
            if (!Cef.IsInitialized) Cef.Initialize(settings);

            OAuthWindow oAuthWindow = new OAuthWindow();
            oAuthWindow.Show();

            authBrowser = oAuthWindow.webBrowser;


            ////webBrowser.Dock = DockStyle.Fill;
            ////webBrowser.NavigateError += new WebBrowserNavigateErrorEventHandler(wb_NavigateError);
            ////Controls.Add(webBrowser);

            //// this is a basic code sample, quick & dirty way to get the Authentication string
            string authorizeURL = FORGE_BASE_URL +
                string.Format("/authentication/v1/authorize?response_type=code&client_id={0}&redirect_uri={1}&scope={2}",
                FORGE_CLIENT_ID, FORGE_CALLBACK_URL, System.Net.WebUtility.UrlEncode(FORGE_SCOPE));


            authBrowser.MinHeight = 600;
            authBrowser.MinWidth = 300;
            authBrowser.LoadUrl(authorizeURL);
            
            //PrepareUserData();
            authBrowser.FrameLoadEnd += Browser_FrameLoadEnd;
            //oAuthWindow.Close();
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            var codePattern = "&code=";

            // if the browser redirect to this page, means the session ID is on the body
            if (!e.Url.Contains(codePattern))
            {
                return;
            }
            else
            {
                // remove this event...
                authBrowser.FrameLoadEnd -= Browser_FrameLoadEnd;

                var code = e.Url.Substring(e.Url.IndexOf(codePattern) + codePattern.Length);
            }
        }

    }


}
