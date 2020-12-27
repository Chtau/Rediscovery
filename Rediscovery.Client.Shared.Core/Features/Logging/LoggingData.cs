using Rediscovery.Shared.Logging.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Client.Shared.Core.Features.Logging
{
    public class LoggingData : ILoggingData
    {
        private ConcurrentQueue<LoggerEntry> entries = new ConcurrentQueue<LoggerEntry>();
        private TimeSpan? timeSpanEntriesAddedEvent;

        public event EventHandler AddedNewEntries;

        public void AddEntry(LoggerEntry loggerEntry)
        {
            entries.Enqueue(loggerEntry);
            try
            {
                if (timeSpanEntriesAddedEvent.HasValue)
                    return;
                timeSpanEntriesAddedEvent = TimeSpan.FromSeconds(15);
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(timeSpanEntriesAddedEvent.Value).ConfigureAwait(false);
                        AddedNewEntries?.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Fail(ex.ToString());
                    } finally
                    {
                        timeSpanEntriesAddedEvent = null;
                    }
                });
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.Fail(ex.ToString());
            }
        }

        public LoggerEntry GetNextEntry()
        {
            if (entries.TryDequeue(out LoggerEntry entry))
                return entry;
            return null;
        }
    }
}
