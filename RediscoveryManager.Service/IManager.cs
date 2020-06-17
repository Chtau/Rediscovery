using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.Service
{
    public interface IManager
    {
        Models.ManagerConnectionState ManagerConnectionState { get; }
        Models.CurrentConnection CurrentConnection { get; }
        void Connect(string ip, int port, string deviceIdentifier);
    }
}
