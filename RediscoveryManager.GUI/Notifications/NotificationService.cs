using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.GUI.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IManagedNotificationManager _notificationManager;

        public NotificationService(Window host)
        {
            _notificationManager = new WindowNotificationManager(host)
            {
                Position = NotificationPosition.TopRight,
                MaxItems = 2
            };
        }

        public void Show(string title, string message, Action<bool> resultCallback)
        {
            _notificationManager.Show(new NotificationViewModel(_notificationManager, resultCallback) { Title = title, Message = message });
        }

        public void Show(string title, string message)
        {
            _notificationManager.Show(new Avalonia.Controls.Notifications.Notification(title, message));
        }
    }
}
