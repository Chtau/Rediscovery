using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rediscovery.Client.App.Manager.GUI.ViewModels
{
    public class HeartbeatStatisticsViewModel : ViewModelBase
    {
        private readonly IManager _manager;
        private readonly SharedBase.Logging.ILogger _logger;

        private ObservableCollection<SharedBase.Statistics.HeartbeatStatisticItem> items = new ObservableCollection<SharedBase.Statistics.HeartbeatStatisticItem>();
        public ObservableCollection<SharedBase.Statistics.HeartbeatStatisticItem> Items
        {
            get { return items; }
            set
            {
                this.RaiseAndSetIfChanged(ref items, value);
            }
        }

        public event EventHandler ItemsChanged;

        public HeartbeatStatisticsViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<SharedBase.Logging.ILogger>();

            _manager.HeartbeatStatisticsChanged += (obj, args) =>
            {
                OnSetItems();
            };
            OnSetItems();
        }

        private void OnSetItems()
        {
            var newCollection = new List<SharedBase.Statistics.HeartbeatStatisticItem>();
            foreach (var item in _manager.HeartbeatStatistics)
            {
                newCollection.Add(item);
            }
            Items = new ObservableCollection<SharedBase.Statistics.HeartbeatStatisticItem>(newCollection);
            ItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
