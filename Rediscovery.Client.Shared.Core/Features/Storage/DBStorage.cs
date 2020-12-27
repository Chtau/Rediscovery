using LiteDB;
using Rediscovery.Client.App.Core.Dependency;
using Rediscovery.Client.App.Core.Features.Storage.Models;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Storage
{
    public class DBStorage : IDBStorage
    {
        private readonly ILogger _logger;
        private readonly ISettingValue<StorageSetting> _monitorSettings;
        private bool disposedValue;

        public LiteDatabase Database { get; private set; }

        public DBStorage(ILogger logger, ISettingValue<StorageSetting> storageSettingValue)
        {
            _logger = logger;
            _monitorSettings = storageSettingValue;
        }

        private ConnectionString OnGetConnectionString()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_monitorSettings.CurrentValue.DatabaseFile))
                    return null;
                ConnectionString connectionString = new ConnectionString();
                connectionString.Filename = _monitorSettings.CurrentValue.DatabaseFile;
                if (!string.IsNullOrWhiteSpace(_monitorSettings.CurrentValue.DatabasePassword))
                    connectionString.Password = _monitorSettings.CurrentValue.DatabasePassword;
                return connectionString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get Database connection string");
            }
            return null;
        }

        private bool OnCreateOrOpenIfClosed()
        {
            try
            {
                if (Database == null)
                {
                    var connection = OnGetConnectionString();
                    if (connection == null)
                        throw new ArgumentNullException("Connection string is null");
                    Database = new LiteDatabase(connection);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create or open Database");
            }
            return false;
        }

        public bool Delete()
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete Database");
            }
            return false;
        }

        private bool OnClose()
        {
            try
            {
                if (Database != null)
                {
                    Database.Checkpoint();
                    Database.Dispose();
                    Database = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Close Database");
            }
            return false;
        }

        public List<T> EntitiesLoad<T>(Expression<Func<T, bool>> predicate)
        {
            try
            {
                if (OnCreateOrOpenIfClosed())
                {
                    var col = Database.GetCollection<T>();
                    return col?.Find(predicate)?.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Load Entities Type:{typeof(T).FullName}");
            }
            return null;
        }

        public bool EntityDelete<T>(Expression<Func<T, bool>> predicate)
        {
            try
            {
                if (OnCreateOrOpenIfClosed())
                {
                    var col = Database.GetCollection<T>();
                    col.DeleteMany(predicate);
                    Database.Checkpoint();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete Entity Type:{typeof(T).FullName}");
            }
            return false;
        }

        public bool EntityInsertBulk<T>(params T[] entities)
        {
            try
            {
                if (OnCreateOrOpenIfClosed())
                {
                    var col = Database.GetCollection<T>();
                    col.InsertBulk(entities);
                    Database.Checkpoint();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Save Entity Type:{typeof(T).FullName}");
            }
            return false;
        }

        public T EntityLoad<T>(Expression<Func<T, bool>> predicate)
        {
            try
            {
                if (OnCreateOrOpenIfClosed())
                {
                    var col = Database.GetCollection<T>();
                    return col.FindOne(predicate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Load Entity Type:{typeof(T).FullName}");
            }
            return default;
        }

        public bool EntityUpdate<T>(T entity)
        {
            try
            {
                if (OnCreateOrOpenIfClosed())
                {
                    var col = Database.GetCollection<T>();
                    col.Update(entity);
                    Database.Checkpoint();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Save Entity Type:{typeof(T).FullName}");
            }
            return false;
        }

        public bool FileDelete<T>(T id)
        {
            try
            {
                if (OnCreateOrOpenIfClosed())
                {
                    var storage = Database.GetStorage<T>();
                    storage.Delete(id);
                    Database.Checkpoint();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete File Id:{id}");
            }
            return false;
        }

        public FileInfo<T> FileLoad<T>(T id)
        {
            try
            {
                if (OnCreateOrOpenIfClosed())
                {
                    var info = new FileInfo<T>();
                    info.Stream = new MemoryStream();
                    var storage = Database.GetStorage<T>();
                    var fileInfo = storage.Download(id, info.Stream);
                    info.Filename = fileInfo.Filename;
                    info.Id = fileInfo.Id;
                    info.MimeType = fileInfo.MimeType;
                    info.CreateDate = fileInfo.UploadDate;
                    return info;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Load File Id:{id}");
            }
            return null;
        }

        public bool FileSave<T>(FileInfo<T> fileInfo)
        {
            try
            {
                if (OnCreateOrOpenIfClosed())
                {
                    var storage = Database.GetStorage<T>();
                    storage.Upload(fileInfo.Id, fileInfo.Filename, fileInfo.Stream);
                    Database.Checkpoint();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Save File Name:{fileInfo.Filename}");
            }
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnClose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
