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
            model.Close += Model_Close;
        }

        private void Model_Close(object sender, bool e)
        {
            Close(e);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
