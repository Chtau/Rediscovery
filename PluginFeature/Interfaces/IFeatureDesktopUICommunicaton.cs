using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Plugin.Interfaces
{
    public interface IFeatureDesktopUICommunicaton
    {
        event EventHandler<string> SendChangesToUI;
        void ReceivedChangesFromUI(string data);
    }
}
