using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace RediscoveryManager.GUI.Pages
{
    public class Logger : UserControl
    {
        private readonly ViewModels.LoggerViewModel model;

        public Logger()
        {
            this.InitializeComponent();

            DataContext = model = new ViewModels.LoggerViewModel();
            model.ItemsChanged += (obj, args) =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var dg1 = this.FindControl<DataGrid>("dataGrid1");
                    dg1.IsReadOnly = true;

                    var collectionView1 = new DataGridCollectionView(model.Items);
                    collectionView1.GroupDescriptions.Add(new DataGridPathGroupDescription("Sid"));
                    dg1.Items = collectionView1;
                });
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
