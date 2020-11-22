using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Rediscovery.Client.App.Manager.GUI.Notifications
{
    public class NotificationView : UserControl
    {
        public NotificationView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
