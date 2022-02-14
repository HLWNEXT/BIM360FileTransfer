using BIM_360_File_Transfer.Models;
using BIM_360_File_Transfer.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BIM_360_File_Transfer.ViewModels
{
    internal class CustomerViewModel
    {
        /// <summary>
        /// Initializes a new instance of the CustomerViewModel class.
        /// </summary>
        public CustomerViewModel() {
            _Customer = new Customer("David");
            UpdateCommand = new CustomerUpdateCommand(this);
        }

        /// <summary>
        /// Gets or sets a System.Boolean value indicating whether the Customer can be updated.
        /// </summary>
        public bool CanUpdate
        {
            get
            {
                if(Customer is null)
                {
                    return false;
                }
                return !String.IsNullOrWhiteSpace(Customer.Name);
            }
        }

        private Customer _Customer;

        /// <summary>
        /// Gets the customer instance.
        /// </summary>
        public Customer Customer
        {
            get { return _Customer; }
        }

        /// <summary>
        /// Gets the UpdateCommand for the ViewModel.
        /// </summary>
        public ICommand UpdateCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Save changes made to the Customer instance.
        /// </summary>
        public void SaveChanges()
        {
            Debug.Assert(false, String.Format("{0} was updated.", Customer.Name));
        }


    }
}
