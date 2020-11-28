using Rediscovery.Shared.Logging;
using Rediscovery.Shared.Logging.Commands;
using Rediscovery.Shared.Logging.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Rediscovery.Communication.Provider.Logger
{
    public class LoggerHandler : ILoggerHandler
    {
        private readonly IDirectLogger _directLogger;

        private List<LoggerEntry> logEntries = new List<LoggerEntry>();
        private LoggerType logLevel = LoggerType.Trace;
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

        private bool OnAllowedLogLevel(LoggerType loggerType)
        {
            return (int)loggerType >= (int)logLevel;
        }

        public LogCommandConfigResult ExecuteCommand(LogCommandConfig logCommandConfig)
        {
            try
            {
                if (logCommandConfig.CommandType == Command.Clear)
                {
                    ClearEntries();
                    return new LogCommandConfigResult
                    {
                        Data = "",
                        Id = logCommandConfig.Id,
                        Result = true
                    };
                } else if (logCommandConfig.CommandType == Command.ChangeLogLevel)
                {
                    if (int.TryParse(logCommandConfig.Data, out int level))
                    {
                        var newLogLevel = (LoggerType)level;
                        logLevel = newLogLevel;
                        ClearEntries();
                        return new LogCommandConfigResult
                        {
                            Data = "",
                            Id = logCommandConfig.Id,
                            Result = true
                        };
                    }
                } else if (logCommandConfig.CommandType == Command.State)
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
