using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using BIM360FileTransfer.Commands;
using BIM360FileTransfer.Interfaces;

namespace BIM360FileTransfer.ViewModels
{
    public delegate void OperateFamily(string familyName);
    public class SourceViewModel : SelectableObject, IModelCore<IContent>
    {
        private readonly Size _thumbnailSize = new Size(100.0, 100.0);

        public IContent Model { get; }

        public SourceViewModel(IContent content)
        {
            Model = content;
            Name = Model.Name;
            if (string.IsNullOrEmpty(Model.ContentLocalPath))
                Status = "Download";
            else Status = "Load";
        }

        private static bool _canLoad = true;
        private static bool _hasLoaded = false;

        public OperateFamily LoadFamily;

        public OperateFamily PlaceFamily;


        private string displayName;
        public string DisplayName
        {
            get => displayName;
            set
            {
                displayName = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return Model.Name;
            }
            set
            {
                Model.Name = value;
                RefreshDisplayName();
                OnPropertyChanged();
            }
        }


        private void RefreshDisplayName()
        {
            if (Name == null)
            {
                DisplayName = null;
                return;
            }

            var text = Name.ToLower();
            var length = text.Length;
            foreach (var text2 in Extensions)
                if (text.EndsWith(text2))
                {
                    DisplayName = Name.Substring(0, length - text2.Length);
                    return;
                }

            DisplayName = Name;
        }

        private string status;

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        public ICommand PlaceCommand { get; }

        public ICommand DownloadCommand { get; }

        private static string[] Extensions { get; } = { ".rfa" };

    }
}
