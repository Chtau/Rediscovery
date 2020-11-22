using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Rediscovery.Client.App.Manager.GUI.Pages
{
    public class Devices : UserControl
    {
        private readonly ViewModels.DevicesViewModel model;

        public Devices()
        {
            this.InitializeComponent();
            DataContext = model = new ViewModels.DevicesViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
