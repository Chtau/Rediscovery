using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Rediscovery.Client.App.Manager.GUI.Pages
{
    public class Status : UserControl
    {
        private readonly ViewModels.StatusViewModel model;

        public Status()
        {
            this.InitializeComponent();
            DataContext = model = new ViewModels.StatusViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
