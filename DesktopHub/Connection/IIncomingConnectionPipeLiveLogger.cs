using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopHub.Connection
{
    public interface IIncomingConnectionPipeLiveLogger
    {
        event EventHandler<SharedCoreModels.LiveLoggerModel> LiveLoggerEntry;
        void ListenForConnections();
    }
}
