using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Rediscovery.Feature.Client.FileExchange.UI.ViewModels;
using System;

namespace Rediscovery.Feature.Client.FileExchange.UI.Views
{
    public class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = viewModel = new MainWindowViewModel((Window)this.VisualRoot);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
