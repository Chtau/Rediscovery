using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Storage
{
    public class DBStorage : IDBStorage
    {
        private readonly StorageSetting _storageSetting;

        public DBStorage(StorageSetting storageSetting)
        {
            _storageSetting = storageSetting;
        }
    }
}
