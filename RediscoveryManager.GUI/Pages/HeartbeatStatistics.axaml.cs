using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using SharedBase.Statistics;
using System;
using System.Collections.Generic;

namespace RediscoveryManager.GUI.Pages
{
    public class HeartbeatStatistics : UserControl
    {
        private readonly ViewModels.HeartbeatStatisticsViewModel model;

        public HeartbeatStatistics()
        {
            this.InitializeComponent();
            DataContext = model = new ViewModels.HeartbeatStatisticsViewModel();
            model.ItemsChanged += (obj, args) =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var dg1 = this.FindControl<DataGrid>("dataGrid1");
                    dg1.IsReadOnly = true;

                    var collectionView1 = new DataGridCollectionView(model.Items);
                    collectionView1.GroupDescriptions.Add(new DataGridPathGroupDescription("DeviceId"));
                    dg1.Items = collectionView1;
                });
            };

            /*var dg1 = this.FindControl<DataGrid>("dataGrid1");
            dg1.IsReadOnly = true;

            var items = new List<HeartbeatStatisticItem>
            {
                new HeartbeatStatisticItem { DeviceId = "1", OK = true, PingPongTime = null, PingStartDatetimeUTC = null, ResultReceived = DateTime.Now},
                new HeartbeatStatisticItem { DeviceId = "1", OK = true, PingPongTime = null, PingStartDatetimeUTC = null, ResultReceived = DateTime.Now},
                new HeartbeatStatisticItem { DeviceId = "1", OK = true, PingPongTime = null, PingStartDatetimeUTC = null, ResultReceived = DateTime.Now}
            };

            var collectionView1 = new DataGridCollectionView(items);
            dg1.Items = collectionView1;*/
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
