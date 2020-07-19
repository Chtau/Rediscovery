using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CommunicationLoggerProvider
{
    public class LoggerHandler : ILoggerHandler
    {
        private readonly IDirectLogger _directLogger;

        private List<LoggerEntry> logEntries = new List<LoggerEntry>();
        private SharedBase.Logging.LoggerEntry.LoggerType logLevel = LoggerEntry.LoggerType.Trace;
        public bool Pause { get; set; }
        public int MaxEntires { get; set; } = 100;
        private DateTime lastEntiresChangedEvent = DateTime.UtcNow.AddMinutes(-1);

        public LoggerHandler(IDirectLogger directLogger)
        {
            _directLogger = directLogger;
        }

        public event EventHandler EntriesChanged;

        public void ClearEntries()
        {
            try
            {
                Pause = true;
                logEntries.Clear();
            }
            catch (Exception ex)
            {
                _directLogger.LogException(ex);
            } finally
            {
                Pause = false;
            }
        }

        public List<LoggerEntry> Get()
        {
            try
            {
                return logEntries;
            }
            catch (Exception ex)
            {
                _directLogger.LogException(ex);
            }
            return new List<LoggerEntry>();
        }

        public void NewEntry(LoggerEntry loggerEntry)
        {
            try
            {
                if (!Pause)
                {
                    if (logEntries.Count > MaxEntires)
                        ClearEntries();
                    if (OnAllowedLogLevel(loggerEntry.LogLevel))
                    {
                        logEntries.Add(loggerEntry);
                        if (lastEntiresChangedEvent.AddSeconds(10) < DateTime.UtcNow)
                        {
                            lastEntiresChangedEvent = DateTime.UtcNow;
                            EntriesChanged?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
            } catch (Exception ex)
            {
                _directLogger.LogException(ex);
            }
        }

        private bool OnAllowedLogLevel(SharedBase.Logging.LoggerEntry.LoggerType loggerType)
        {
            return (int)loggerType >= (int)logLevel;
        }

        public LogCommandConfigResult ExecuteCommand(LogCommandConfig logCommandConfig)
        {
            try
            {
                if (logCommandConfig.CommandType == LogCommandConfig.Command.Clear)
                {
                    ClearEntries();
                    return new LogCommandConfigResult
                    {
                        Data = "",
                        Id = logCommandConfig.Id,
                        Result = true
                    };
                } else if (logCommandConfig.CommandType == LogCommandConfig.Command.ChangeLogLevel)
                {
                    if (int.TryParse(logCommandConfig.Data, out int level))
                    {
                        var newLogLevel = (SharedBase.Logging.LoggerEntry.LoggerType)level;
                        logLevel = newLogLevel;
                        ClearEntries();
                        return new LogCommandConfigResult
                        {
                            Data = "",
                            Id = logCommandConfig.Id,
                            Result = true
                        };
                    }
                } else if (logCommandConfig.CommandType == LogCommandConfig.Command.State)
                {
                    var state = new LoggerState
                    {
                        Level = logLevel
                    };
                    return new LogCommandConfigResult
                    {
                        Data = Newtonsoft.Json.JsonConvert.SerializeObject(state),
                        Id = logCommandConfig.Id,
                        Result = true
                    };
                }
            }
            catch (Exception ex)
            {
                _directLogger.LogException(ex);
            }
            return new LogCommandConfigResult
            {
                Data = "",
                Id = logCommandConfig.Id,
                Result = false
            };
        }
    }
}
