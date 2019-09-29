using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.DeviceFeature
{
    public interface IDeviceFeatureImplementation<T>
    {
        void Init();
        void Dispose();
        event EventHandler<T> SendData;
        void ReceiveData(T data);
    }
}
