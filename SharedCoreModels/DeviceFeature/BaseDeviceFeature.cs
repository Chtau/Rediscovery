using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels.DeviceFeature
{
    public abstract class BaseDeviceFeature : IDeviceFeatureImplementation
    {
        public event EventHandler<DeviceFeatureData> SendData;

        public void OnSendData(object obj, DeviceFeatureData args)
        {
            SendData?.Invoke(obj, args);
        }

        public virtual void Dispose()
        {
            
        }

        public virtual DeviceFeature GetDeviceFeatureInfo()
        {
            return null;
        }

        public virtual void Init()
        {
            
        }

        public virtual void ReceiveData(DeviceFeatureData data)
        {
            
        }

        public virtual void Register(string deviceId)
        {
            
        }

        public virtual void Unregister(string deviceId)
        {
            
        }
    }
}
