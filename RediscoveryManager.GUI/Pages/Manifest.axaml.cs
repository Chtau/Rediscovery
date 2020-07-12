using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RediscoveryManager.GUI.Pages
{
    public class Manifest : UserControl
    {
        public Manifest()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
