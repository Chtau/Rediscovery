using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public interface IActiveDeviceService
    {
        void AddActiveDevice(string userId);
        void RemoveActiveDevice(string userId);
    }
}
