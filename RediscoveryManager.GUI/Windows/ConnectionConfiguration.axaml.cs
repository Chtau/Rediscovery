using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RediscoveryManager.GUI.ViewModels;

namespace RediscoveryManager.GUI.Windows
{
    public class ConnectionConfiguration : Window
    {
        private readonly ConnectionConfigurationViewModel model;

        public ConnectionConfiguration()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public ConnectionConfiguration(ConnectionConfigurationViewModel viewModel) : this()
        {
            DataContext = model = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
