using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Client.Shared.Core.Features.Logging;
using Rediscovery.Client.Shared.Core.Features.Storage;
using Rediscovery.Shared.Logging;
using Rediscovery.Shared.Logging.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Client.Shared.Core.Test
{
    public class LoggerTest
    {
        [Fact]
        public async void LogData()
        {
            CoreManager.Init(new CoreManagerSetting
            {
                CurrentStorageSetting = new StorageSetting
                {
                    DatabaseFile = null
                }
            });

            bool entriesReceived = false;
            var loggingData = Resolver.Get<ILoggingData>();
            var logger = Resolver.Get<ILogger>();
            loggingData.AddedNewEntries += (obj, args) =>
            {
                entriesReceived = true;
            };
            logger.LogCritical("Test1");
            logger.LogCritical("Test2");
            logger.LogInformation("Test3");
            do
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10));
            } while (!entriesReceived);
            
            var entires = new List<LoggerEntry>();
            LoggerEntry entry = null;
            do
            {
                entry = loggingData.GetNextEntry();
                if (entry != null)
                    entires.Add(entry);
            } while (entry != null);
            Assert.True(entires.Count == 3);
        }
    }
}
