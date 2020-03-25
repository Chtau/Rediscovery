using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.DBStore))]
namespace Rediscovery.Services
{
    public class DBStore : BaseService, IDBStore
    {
        public SQLiteAsyncConnection Store { get; }

        public DBStore()
        {
            try
            {
                Store = new SQLiteAsyncConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "rediscovery.db3"));
                OnCreateTables().GetAwaiter();
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private async Task OnCreateTables()
        {
            await Store.CreateTableAsync<Features.Connection.Models.ConnectionInfo>();
            await Store.CreateTableAsync<Features.Connection.Models.ConnectionManifestFeature>();
        }
    }
}
