using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.DeviceFeature
{
    public interface IDeviceFeatureImplementation
    {
        void Init();
        void Dispose();
        event EventHandler<DeviceFeatureData> SendData;
        void ReceiveData(DeviceFeatureData data);
        DeviceFeature GetDeviceFeatureInfo();
        void Start();
        void Stop();
    }
}
