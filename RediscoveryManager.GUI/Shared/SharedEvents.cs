using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.Shared
{
    public class SharedEvents : ISharedEvents
    {
        private object lastInstance;
        private bool lastState;

        public event EventHandler<bool> LoadingState;

        public object GetInstance()
        {
            return lastInstance;
        }

        public bool HasLoadingState()
        {
            return lastState;
        }

        public void InvokeLoading(object instance, bool state)
        {
            lastState = state;
            if (state)
                lastInstance = instance;
            else
                lastInstance = null;
            LoadingState?.Invoke(instance, state);
        }
    }
}
