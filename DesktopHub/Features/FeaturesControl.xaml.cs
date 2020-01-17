using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DesktopHub.Features
{
    public class FeaturesControl : UserControl
    {
        private readonly FeaturesControlViewModel _viewModel;

        public FeaturesControl()
        {
            this.InitializeComponent();

            DataContext = _viewModel = new FeaturesControlViewModel();
            _viewModel.Refresh();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
