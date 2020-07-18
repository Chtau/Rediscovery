using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RediscoveryManager.GUI.ViewModels;

namespace RediscoveryManager.GUI.Windows
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
