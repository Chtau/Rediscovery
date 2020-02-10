using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Pipes
{
    public interface IPipeServiceInfo
    {
        void ShowInfoWindow(bool forceStart = false);
    }
}
