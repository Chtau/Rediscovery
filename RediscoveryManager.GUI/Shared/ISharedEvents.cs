using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.Shared
{
    public interface ISharedEvents
    {
        bool HasLoadingState();
        object GetInstance();
        event EventHandler<bool> LoadingState;
        void InvokeLoading(object instance, bool state);
    }
}
