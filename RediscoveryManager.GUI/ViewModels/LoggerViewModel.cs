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

        public class LogLevelComboBox
        {
            public string Name { get; set; }
            public int Level { get; set; }
        }

        private ObservableCollection<LogLevelComboBox> logLevels = new ObservableCollection<LogLevelComboBox>();
        public ObservableCollection<LogLevelComboBox> LogLevels
        {
            get { return logLevels; }
            set
            {
                this.RaiseAndSetIfChanged(ref logLevels, value);
            }
        }

        private LogLevelComboBox currentLevel;
        public LogLevelComboBox CurrentLevel
        {
            get { return currentLevel; }
            set
            {
                this.RaiseAndSetIfChanged(ref currentLevel, value);
                OnAfterLogLevelChanged();
            }
        }

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

            var ll = new List<LogLevelComboBox>();
            ll.Add(new LogLevelComboBox { Level = 0, Name = "Trace" });
            ll.Add(new LogLevelComboBox { Level = 1, Name = "Debug" });
            ll.Add(new LogLevelComboBox { Level = 2, Name = "Information" });
            ll.Add(new LogLevelComboBox { Level = 3, Name = "Warning" });
            ll.Add(new LogLevelComboBox { Level = 4, Name = "Error" });
            ll.Add(new LogLevelComboBox { Level = 5, Name = "Critical" });
            LogLevels = new ObservableCollection<LogLevelComboBox>(ll);

            _manager.LoggerCommandExecuted += (obj, args) =>
            {
                System.Diagnostics.Debug.Print($"Logger command executed Id:{args.Id} Result:{args.Result}");
            };
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

        private void OnAfterLogLevelChanged()
        {
            try
            {
                UpdatePause = true;
                _manager.RemoteLogExecuteCommand(new SharedBase.Logging.LogCommandConfig
                {
                    Id = Guid.NewGuid(),
                    CommandType = SharedBase.Logging.LogCommandConfig.Command.ChangeLogLevel,
                    Data = CurrentLevel.Level.ToString()
                });
                Items = new ObservableCollection<SharedBase.Logging.LoggerEntry>();
                ItemsChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            finally
            {
                UpdatePause = false;
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
                _manager.RemoteLogExecuteCommand(new SharedBase.Logging.LogCommandConfig
                {
                    Id = Guid.NewGuid(),
                    CommandType = SharedBase.Logging.LogCommandConfig.Command.Clear
                });
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
