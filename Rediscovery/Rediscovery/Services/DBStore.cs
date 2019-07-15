using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.DBStore))]
namespace Rediscovery.Services
{
    public class DBStore : IDBStore
    {
        public SQLiteAsyncConnection Store { get; }

        public DBStore()
        {
            Store = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "rediscovery.db3"));
        }
    }
}
