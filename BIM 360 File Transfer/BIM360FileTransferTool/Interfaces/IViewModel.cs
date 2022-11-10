using System;

namespace BIM360FileTransfer.Interfaces
{
    public interface IViewModel
    {
        string Id { get; }
        string Name { get; }
        void TryInvoke(Action action);
    }
}
