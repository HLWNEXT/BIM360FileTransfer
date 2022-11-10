using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIM360FileTransfer.Models
{
    public class AuthModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the AuthModel class.
        /// </summary>
        /// <param name="authCode"></param>
        public AuthModel(string authCode)
        {
            Code = authCode;
        }

        public String _Code;

        /// <summary>
        /// Gets or sets the Authentication code.
        /// </summary>
        public String Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                OnPropertyChanged("AuthCode");
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion  

    }
}
