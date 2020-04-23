using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.DBStore))]
namespace Rediscovery.Services
{
    [Obsolete("replace with JSON store")]
    public class DBStore : BaseService, IDBStore
    {
        public SQLiteAsyncConnection Store { get; }

        public DBStore()
        {
            try
            {
                // TODO: remove SQLite we should replace it if a simple JSON
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
            await Store.CreateTableAsync<Features.Settings.Models.SettingModel>();
        }
    }
}
