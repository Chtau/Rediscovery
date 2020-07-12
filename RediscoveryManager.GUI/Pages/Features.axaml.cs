using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RediscoveryManager.GUI.Pages
{
    public class Features : UserControl
    {
        public Features()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
