using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;

        internal SQLiteAsyncConnection DB = null;

        public DBContext(ILoggerFactory loggerFactory, IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> options)
        {
            _logger = loggerFactory.CreateLogger<DBContext>();
            _appSettings = options.Value;
        }

        public async Task Connect(string connectionString)
        {
            try
            {
                DB = new SQLiteAsyncConnection(connectionString);
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
