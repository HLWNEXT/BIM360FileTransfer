//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using Autodesk.Forge;
//using Autodesk.Forge.Client;
//using Autodesk.Forge.Model;

//namespace BIM_360_File_Transfer.FileTransferViewModel
//{
//    public class ViewModelBase
//    {
//        private const string FORGE_CLIENT_ID = "Oc1Dgsd4bxY5hbfvYOsuHCkZTyI1ef7q";
//        private const string FORGE_CLIENT_SECRET = "PwA4iw3Ant6MlkQZ";
//        private const string FORGE_CALLBACK_URL = "http://sampleapp.com/oauth/callback?foo=bar";
//        private const string FORGE_BASE_URL = "https://developer.api.autodesk.com";
//        private const string FORGE_SCOPE = "data:read data:write data:create data:search bucket:create bucket:read bucket:update bucket:delete"; // Full scope access.

//        public ViewModelBase()
//        {

//        }

//        private ICommand _clickCommand;
//        public ICommand ClickCommand
//        {
//            get
//            {
//                return _clickCommand ?? (_clickCommand = new CommandHandler(() => MyAction(), () => CanExecute));
//            }
//        }
//        public bool CanExecute
//        {
//            get
//            {
//                // check if executing is allowed, i.e., validate, check if a process is running, etc. 
//                return true;
//            }
//        }

//        public void MyAction()
//        {

//        }

//    }
//}
