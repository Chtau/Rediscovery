using System;
using System.Reactive;
using Avalonia.Controls.Notifications;
using ReactiveUI;


namespace RediscoveryManager.GUI.Notifications
{
    public class NotificationViewModel
    {
        public NotificationViewModel(INotificationManager manager, Action<bool> resultCallback)
        {
            NoCommand = ReactiveCommand.Create(() =>
            {
                resultCallback?.Invoke(false);
            });
            YesCommand = ReactiveCommand.Create(() =>
            {
                resultCallback?.Invoke(true);
            });
        }

        public string Title { get; set; }
        public string Message { get; set; }


        public ReactiveCommand<Unit, Unit> NoCommand { get; }
        public ReactiveCommand<Unit, Unit> YesCommand { get; }
    }
}
