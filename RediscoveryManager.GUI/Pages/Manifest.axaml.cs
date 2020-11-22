using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Rediscovery.Client.App.Manager.GUI.Pages
{
    public class Manifest : UserControl
    {
        private readonly ViewModels.ManifestViewModel model;

        public Manifest()
        {
            this.InitializeComponent();
            DataContext = model = new ViewModels.ManifestViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
