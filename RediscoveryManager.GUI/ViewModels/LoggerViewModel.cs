using ReactiveUI;
using RediscoveryManager.Service;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

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

        private bool updatePause;
        public bool UpdatePause
        {
            get { return updatePause; }
            set
            {
                this.RaiseAndSetIfChanged(ref updatePause, value);
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
            if (!updatePause)
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

        public void Pause()
        {
            try
            {
                UpdatePause = !UpdatePause;
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void ClearLog()
        {
            try
            {
                UpdatePause = true;
                Items = new ObservableCollection<SharedBase.Logging.LoggerEntry>();
                ItemsChanged?.Invoke(this, EventArgs.Empty);
                // TODO: send a message to clear the log on the service
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            } finally
            {
                UpdatePause = false;
            }
        }

        public void ShowDetail(SharedBase.Logging.LoggerEntry loggerEntry)
        {
            try
            {
                var model = new LoggerEntryViewModel
                {
                    Id = loggerEntry.Id,
                    LogLevel = loggerEntry.LogLevel,
                    Message = loggerEntry.Message,
                    Module = loggerEntry.Module,
                    Sid = loggerEntry.Sid,
                    Time = loggerEntry.Time
                };
                var logEntryDialog = new Windows.LoggerEntry(model);
                logEntryDialog.Show();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
