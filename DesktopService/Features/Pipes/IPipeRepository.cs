using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Pipes
{
    public interface IPipeRepository
    {
        void Init();
        void ActiveDeviceInfoChanged();
        void DeviceInfoChanged();
        void FeatureChanged();
    }
}
