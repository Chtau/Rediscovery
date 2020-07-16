using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationLoggerProvider
{
    public class LoggerHandler : ILoggerHandler
    {
        private readonly IDirectLogger _directLogger;

        public LoggerHandler(IDirectLogger directLogger)
        {
            _directLogger = directLogger;
        }

        public event EventHandler<List<LoggerEntry>> EntriesChanged;

        public List<LoggerEntry> Get()
        {
            try
            {

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

            } catch (Exception ex)
            {
                _directLogger.LogException(ex);
            }
        }
    }
}
