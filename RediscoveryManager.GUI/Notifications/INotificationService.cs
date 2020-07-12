using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.GUI.Notifications
{
    public interface INotificationService
    {
        void Show(string title, string message, Action<bool> resultCallback);
        void Show(string title, string message);
    }
}
