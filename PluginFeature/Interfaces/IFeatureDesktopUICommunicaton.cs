using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature.Interfaces
{
    public interface IFeatureDesktopUICommunicaton
    {
        event EventHandler<string> UISendChanges;
        void UIReceivedChanges(string data);
    }
}
