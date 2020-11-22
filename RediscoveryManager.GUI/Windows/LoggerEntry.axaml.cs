using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Rediscovery.Client.App.Manager.GUI.ViewModels;

namespace Rediscovery.Client.App.Manager.GUI.Windows
{
    public class LoggerEntry : Window
    {
        private readonly LoggerEntryViewModel model;

        public LoggerEntry()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public LoggerEntry(LoggerEntryViewModel viewModel) : this()
        {
            DataContext = model = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
