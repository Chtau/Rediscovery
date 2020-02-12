using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DesktopHub.Features.LiveLogger
{
    public class LiveLoggerControl : UserControl
    {
        private readonly LiveLoggerViewModel _viewModel;

        public LiveLoggerControl()
        {
            this.InitializeComponent();

            DataContext = _viewModel = new LiveLoggerViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
