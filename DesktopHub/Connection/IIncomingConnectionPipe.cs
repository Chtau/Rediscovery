using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopHub.Connection
{
    public interface IIncomingConnectionPipe
    {
        event EventHandler<SharedCoreModels.IncomingConnectionInfo> NewConnectionInfo;
        void ListenForConnections();
    }
}
