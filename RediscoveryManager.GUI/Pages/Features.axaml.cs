using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RediscoveryManager.GUI.Pages
{
    public class Features : UserControl
    {
        private readonly ViewModels.FeaturesViewModel model;

        public Features()
        {
            this.InitializeComponent();
            DataContext = model = new ViewModels.FeaturesViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
