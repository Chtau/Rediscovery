using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature.Interfaces
{
    public interface IFeatureDesktopUICommunicaton
    {
        event EventHandler<string> SendChangesToUI;
        void ReceivedChangesFromUI(string data);
    }
}
