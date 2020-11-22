using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Rediscovery.Client.App.Manager.GUI.Pages
{
    public class ActiveDevices : UserControl
    {
        private readonly ViewModels.ActiveDevicesViewModel model;

        public ActiveDevices()
        {
            this.InitializeComponent();
            DataContext = model = new ViewModels.ActiveDevicesViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
