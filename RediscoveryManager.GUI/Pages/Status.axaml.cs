using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RediscoveryManager.GUI.Pages
{
    public class Status : UserControl
    {
        public Status()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
