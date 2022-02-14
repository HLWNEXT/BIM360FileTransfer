using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIM_360_File_Transfer.Models
{
    public class Customer : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the Customer class.
        /// </summary>
        /// <param name="customerName"></param>
        public Customer(string customerName)
        {
            Name = customerName;
        }

        public String _Name;
        /// <summary>
        /// Gets or sets the Customer's name.
        /// </summary>
        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
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
