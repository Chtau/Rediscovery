using Rediscovery.Shared.Base.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Heartbeat.Provider
{
    public interface IHeartbeatActive
    {
        event EventHandler ActiveSIDsChanged;
        List<string> GetActiveSIDs();
        HeartbeatActiveDeviceDetail Detail(string sid);
        void TryAdd(HeartbeatActiveDeviceDetail deviceDetail);
        void TryRemove(string sid);
    }
}
