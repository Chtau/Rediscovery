using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Client.App.Manager.GUI.ViewModels
{
    public class LoggerViewModel : ViewModelBase
    {
        private readonly IManager _manager;
        private readonly Rediscovery.Shared.Logging.ILogger _logger;

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
                if (!internalLogLevelChange)
                    OnAfterLogLevelChanged();
            }
        }

        private ObservableCollection<Rediscovery.Shared.Logging.Models.LoggerEntry> items = new ObservableCollection<Rediscovery.Shared.Logging.Models.LoggerEntry>();
        public ObservableCollection<Rediscovery.Shared.Logging.Models.LoggerEntry> Items
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

        private Guid stateCommandId = Guid.NewGuid();
        private bool internalLogLevelChange = false;

        public LoggerViewModel()
        {
            _manager = Locator.Current.GetService<IManager>();
            _logger = Locator.Current.GetService<Rediscovery.Shared.Logging.ILogger>();

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
                if (args.Id == stateCommandId)
                {
                    var loggerState = Newtonsoft.Json.JsonConvert.DeserializeObject<Rediscovery.Shared.Logging.Models.LoggerState>(args.Data);
                    if (loggerState != null)
                    {
                        var llItem = LogLevels.FirstOrDefault(x => x.Level == (int)loggerState.Level);
                        if (llItem != null)
                        {
                            internalLogLevelChange = true;
                            CurrentLevel = llItem;
                            internalLogLevelChange = false;
                        }
                    }
                }
            };
            _manager.LoggerEntiresChanged += (obj, args) =>
            {
                OnSetItems();
            };
            _manager.AfterConnecting += (obj, args) =>
            {
                _manager.RemoteLogExecuteCommand(new Rediscovery.Shared.Logging.Commands.LogCommandConfig
                {
                    CommandType = Rediscovery.Shared.Logging.Command.State,
                    Id = stateCommandId
                });
            };
            
            OnSetItems();
        }

        private void OnSetItems()
        {
            if (!updatePause)
            {
                var newCollection = new List<Rediscovery.Shared.Logging.Models.LoggerEntry>();
                foreach (var item in _manager.LoggerEntires)
                {
                    newCollection.Add(item);
                }
                Items = new ObservableCollection<Rediscovery.Shared.Logging.Models.LoggerEntry>(newCollection);
                ItemsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnAfterLogLevelChanged()
        {
            try
            {
                UpdatePause = true;
                _manager.RemoteLogExecuteCommand(new Rediscovery.Shared.Logging.Commands.LogCommandConfig
                {
                    Id = Guid.NewGuid(),
                    CommandType = Rediscovery.Shared.Logging.Command.ChangeLogLevel,
                    Data = CurrentLevel.Level.ToString()
                });
                Items = new ObservableCollection<Rediscovery.Shared.Logging.Models.LoggerEntry>();
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
                _manager.RemoteLogExecuteCommand(new Rediscovery.Shared.Logging.Commands.LogCommandConfig
                {
                    Id = Guid.NewGuid(),
                    CommandType = Rediscovery.Shared.Logging.Command.Clear
                });
                Items = new ObservableCollection<Rediscovery.Shared.Logging.Models.LoggerEntry>();
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

        public void ShowDetail(Rediscovery.Shared.Logging.Models.LoggerEntry loggerEntry)
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
