using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.RemoteResources
{
    public interface IRemoteResourcesRepository
    {
        void Init();
        void ActiveDeviceInfoChanged();
        void DeviceInfoChanged();
        void FeatureChanged();
    }
}
