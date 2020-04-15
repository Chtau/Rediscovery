using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AppControlPanel.Views
{
    public class MainWindow : Window
    {
        private CheckBox topMostCheckbox;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            topMostCheckbox = this.FindControl<CheckBox>("topMostCheck");
            topMostCheckbox.Click += TopMostCheckbox_Click;
        }

        private void TopMostCheckbox_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Topmost = topMostCheckbox.IsChecked.Value;
        }
    }
}
