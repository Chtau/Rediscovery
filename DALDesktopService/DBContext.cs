using Microsoft.Extensions.Logging;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DALDesktopService
{
    public class DBContext : IDBContext
    {
        private readonly ILogger<DBContext> _logger;

        internal SQLiteAsyncConnection DB = null;

        public DBContext(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DBContext>();
            Connect().GetAwaiter();
        }

        public async Task Connect()
        {
            try
            {
                DB = new SQLiteAsyncConnection(ConfigurationInstance.Configuration.ConnectionString);
                await OnBuildModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task OnBuildModel()
        {
            await DB.CreateTableAsync<Models.Device>();
            await DB.CreateTableAsync<Models.DevicePendingAuthentication>();
        }

        public SQLiteAsyncConnection Instance
        {
            get
            {
                return DB;
            }
        }
    }
}
