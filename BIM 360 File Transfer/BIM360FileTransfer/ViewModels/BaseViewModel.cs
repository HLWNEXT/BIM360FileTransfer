using BIM360FileTransfer.Interfaces;
using System;

namespace BIM360FileTransfer.ViewModels
{
    public abstract class BaseViewModel : ObserverableObject, IViewModel
    {
        public string Id { get; }

        public string Name => GetType().Name;

        public void TryInvoke(Action action){}
    }
}
