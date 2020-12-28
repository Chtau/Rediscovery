using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Client.Shared.Core.Features.Storage;
using Rediscovery.Shared.Logging.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Rediscovery.Client.Shared.Core.Test
{
    public class StorageTest
    {
        [Fact]
        public void DBStorage()
        {
            string dbFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tmp.db");
            string id = Guid.NewGuid().ToString();
            string sid = Guid.NewGuid().ToString();
            CoreManager.Init(new CoreManagerSetting
            {
                CurrentStorageSetting = new StorageSetting
                {
                    DatabaseFile = dbFile
                }
            });
            var storage = Resolver.Get<IDBStorage>();
            Assert.True(storage.EntityInsertBulk<LoggerEntry>(new LoggerEntry
            {
                Id = id,
                LogLevel = Rediscovery.Shared.Logging.LoggerType.Critical,
                Message = "Test1",
                Module = "T",
                Sid = sid,
                Time = DateTime.Now,
            }));
            Assert.NotNull(storage.EntityLoad<LoggerEntry>(x => x.Id == id));
            Assert.True(storage.EntitiesLoad<LoggerEntry>(x => x.Sid == sid).Count == 1);
            Assert.True(storage.EntityUpdate<LoggerEntry>(new LoggerEntry
            {
                Id = id,
                LogLevel = Rediscovery.Shared.Logging.LoggerType.Critical,
                Message = "Test1_update",
                Module = "T",
                Sid = sid,
                Time = DateTime.Now,
            }));
            Assert.Equal("Test1_update", storage.EntityLoad<LoggerEntry>(x => x.Id == id).Message);
            Assert.True(storage.EntityDelete<LoggerEntry>(x => x.Id == id));

            string storeFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tmp.json");
            var jSONStorage = Resolver.Get<IJSONStorage>();
            Assert.True(jSONStorage.SetFileContent(new LoggerEntry
            {
                Id = id,
                LogLevel = Rediscovery.Shared.Logging.LoggerType.Critical,
                Message = "Test1",
                Module = "T",
                Sid = sid,
                Time = DateTime.Now,
            }, storeFile));

            Assert.True(storage.FileSave<string>(new Features.Storage.Models.FileInfo<string>
            {
                Id = id,
                CreateDate = DateTime.Now,
                Filename = "Test.json",
                MimeType = "plain/txt",
                Stream = new System.IO.FileStream(storeFile, System.IO.FileMode.Open)
            }));
            Assert.Equal("Test.json", storage.FileLoad<string>(id).Filename);
            Assert.True(storage.FileDelete<string>(id));
        }

        [Fact]
        public void JSONStorage()
        {
            string storeFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "tmp.json");
            string id = Guid.NewGuid().ToString();
            string sid = Guid.NewGuid().ToString();
            CoreManager.Init(new CoreManagerSetting
            {
                CurrentStorageSetting = new StorageSetting
                {
                    DatabaseFile = null
                }
            });
            var storage = Resolver.Get<IJSONStorage>();
            Assert.True(storage.SetFileContent(new LoggerEntry
            {
                Id = id,
                LogLevel = Rediscovery.Shared.Logging.LoggerType.Critical,
                Message = "Test1",
                Module = "T",
                Sid = sid,
                Time = DateTime.Now,
            }, storeFile));
            Assert.Equal(id, storage.GetFileContent<LoggerEntry>(storeFile).Id);
            Assert.True(storage.DeleteFile(storeFile));
        }
    }
}
