using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Splat;

namespace Rediscovery.Client.App.Manager.GUI.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
