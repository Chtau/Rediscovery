using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.Notifications
{
    public interface INotificationService
    {
        void Show(string title, string message, Action<bool> resultCallback);
        void Show(string title, string message);
    }
}
