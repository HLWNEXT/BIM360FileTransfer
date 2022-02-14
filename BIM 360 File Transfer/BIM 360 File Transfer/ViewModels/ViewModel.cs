using BIM_360_File_Transfer.Models;
using BIM_360_File_Transfer.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using BIM_360_File_Transfer.VIews;

namespace BIM_360_File_Transfer.ViewModels
{
    internal class ViewModel
    {
        private const string FORGE_CLIENT_ID = "Oc1Dgsd4bxY5hbfvYOsuHCkZTyI1ef7q";
        private const string FORGE_CLIENT_SECRET = "PwA4iw3Ant6MlkQZ";
        private const string FORGE_CALLBACK_URL = "http://sampleapp.com/oauth/callback?foo=bar";
        private const string FORGE_BASE_URL = "https://developer.api.autodesk.com";
        private const string FORGE_SCOPE = "data:read data:write data:create data:search bucket:create bucket:read bucket:update bucket:delete"; // Full scope access.




        /// <summary>
        /// Initializes a new instance of the CustomerViewModel class.
        /// </summary>
        public ViewModel()
        {
            _AuthModel = new AuthModel("");
            OpenAuthCommand = new AuthCommand(this);
        }

        /// <summary>
        /// Gets or sets a System.Boolean value indicating whether the Customer can be updated.
        /// </summary>
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

        /// <summary>
        /// Gets the customer instance.
        /// </summary>
        public AuthModel AuthModel
        {
            get { return _AuthModel; }
        }

        /// <summary>
        /// Gets the UpdateCommand for the ViewModel.
        /// </summary>
        public ICommand OpenAuthCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Save changes made to the Customer instance.
        /// </summary>
        public void OpenAuthPage()
        {
            OAuthWindow oAuthWindow = new OAuthWindow();
            oAuthWindow.Show();

            
            ////webBrowser.Dock = DockStyle.Fill;
            ////webBrowser.NavigateError += new WebBrowserNavigateErrorEventHandler(wb_NavigateError);
            ////Controls.Add(webBrowser);

            //// this is a basic code sample, quick & dirty way to get the Authentication string
            string authorizeURL = FORGE_BASE_URL + 
                string.Format("/authentication/v1/authorize?response_type=code&client_id={0}&redirect_uri={1}&scope={2}",
                FORGE_CLIENT_ID, FORGE_CALLBACK_URL, System.Net.WebUtility.UrlEncode(FORGE_SCOPE));

            //// now let's open the Authorize page.
            oAuthWindow.webBrowser.LoadUrl(authorizeURL);
        }

    }
}
