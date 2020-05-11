using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.RemoteResources
{
    public interface IRemoteResourcesServiceInfo
    {
        [Obsolete("Refactor")]
        void ShowInfoWindow(bool forceStart = false);
    }
}
