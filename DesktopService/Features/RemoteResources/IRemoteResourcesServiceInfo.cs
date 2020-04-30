using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.RemoteResources
{
    public interface IRemoteResourcesServiceInfo
    {
        void ShowInfoWindow(bool forceStart = false);
    }
}
