using ReactiveUI;
using RediscoveryManager.Service;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RediscoveryManager.GUI.ViewModels
{
    public class LoggerViewModel : ViewModelBase
    {
        private readonly IManager _manager;
        private readonly SharedBase.Logging.ILogger _logger;

        private ObservableCollection<SharedBase.Logging.LoggerEntry> items = new ObservableCollection<SharedBase.Logging.LoggerEntry>();
        public ObservableCollection<SharedBase.Logging.LoggerEntry> Items
        {
            get { return items; }
            set
            {
                this.RaiseAndSetIfChanged(ref items, value);
            }
        }

        public event EventHandler ItemsChanged;

        public LoggerViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<SharedBase.Logging.ILogger>();

            _manager.LoggerEntiresChanged += (obj, args) =>
            {
                OnSetItems();
            };
            OnSetItems();
        }

        private void OnSetItems()
        {
            var newCollection = new List<SharedBase.Logging.LoggerEntry>();
            foreach (var item in _manager.LoggerEntires)
            {
                newCollection.Add(item);
            }
            Items = new ObservableCollection<SharedBase.Logging.LoggerEntry>(newCollection);
            ItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
