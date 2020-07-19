using SharedBase.Device;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationHeartbeatProvider
{
    public interface IHeartbeatActive
    {
        event EventHandler ActiveSIDsChanged;
        List<string> GetActiveSIDs();
        HeartbeatActiveDeviceDetail Detail(string sid);
        void TryAdd(HeartbeatActiveDeviceDetail deviceDetail);
    }
}
