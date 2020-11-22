using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace Rediscovery.Client.App.Manager.GUI.Pages
{
    public class Logger : UserControl
    {
        private readonly ViewModels.LoggerViewModel model;

        private int lastClickIndex = -1;

        public Logger()
        {
            this.InitializeComponent();

            DataContext = model = new ViewModels.LoggerViewModel();
            model.ItemsChanged += (obj, args) =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var dg1 = this.FindControl<DataGrid>("dataGrid1");
                    dg1.IsReadOnly = false;
                    dg1.BeginningEdit += (obj, args) =>
                    {
                        var curIndex = args.Row.GetIndex();
                        if (curIndex != lastClickIndex)
                        {
                            lastClickIndex = curIndex;
                            var entry = args.Row.DataContext as SharedBase.Logging.LoggerEntry;
                            if (entry != null)
                            {
                                model.ShowDetail(entry);
                            }
                        }
                        args.Cancel = true;
                    };
                    
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
