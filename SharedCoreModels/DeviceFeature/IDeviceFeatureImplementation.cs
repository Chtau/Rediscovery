using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.DeviceFeature
{
    public interface IDeviceFeatureImplementation
    {
        void Init();
        void Dispose();
        event EventHandler<object> SendData;
        void ReceiveData(object data);
        DeviceFeature GetDeviceFeatureInfo();
    }
}
