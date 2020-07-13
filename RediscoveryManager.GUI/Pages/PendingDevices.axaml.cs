using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RediscoveryManager.GUI.Pages
{
    public class PendingDevices : UserControl
    {
        private readonly ViewModels.PendingDevicesViewModel model;

        public PendingDevices()
        {
            this.InitializeComponent();
            DataContext = model = new ViewModels.PendingDevicesViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
