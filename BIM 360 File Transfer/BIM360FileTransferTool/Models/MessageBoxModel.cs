using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BIM360FileTransfer.Models
{
    internal class MessageBoxModel
    {
        public string messageBoxText {get;set;}
        public string caption { get; set; }
        public MessageBoxButton button { get; set;}
        public MessageBoxImage icon { get; set; }
        public MessageBoxResult result { get; set; }

        public MessageBoxModel(string _messageBoxText, string _caption, MessageBoxButton _button, MessageBoxImage _icon)
        {
            messageBoxText = _messageBoxText;
            caption = _caption;
            button = _button;
            icon = _icon;
            result = new MessageBoxResult();
        }
    }
}
