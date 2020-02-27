using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.DAL
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
            Connect().GetAwaiter();
        }

        public async Task Connect()
        {
            try
            {
                DB = new SQLiteAsyncConnection(System.IO.Path.Combine(AppFolders.GetUserFolder(_appSettings.AppDataFolder), "rediscovery.db"));
                await OnBuildModel();
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task OnBuildModel()
        {
            await DB.CreateTableAsync<Features.Identity.Models.Device>();
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
