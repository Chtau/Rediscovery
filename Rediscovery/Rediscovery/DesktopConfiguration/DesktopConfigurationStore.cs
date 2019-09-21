using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.DesktopConfiguration.DesktopConfigurationStore))]
namespace Rediscovery.DesktopConfiguration
{
    public class DesktopConfigurationStore : IDataStoreGuid<DesktopConfigurationModel>
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();

        private IDataStoreGuid<Features.Authentication.Models.Connection> connectionStore => DependencyService.Get<IDataStoreGuid<Features.Authentication.Models.Connection>>() ?? new Features.Authentication.ConnectionStore();

        public async Task<bool> AddItemAsync(DesktopConfigurationModel item)
        {
            var con = await connectionStore.GetItemAsync(item.Id);
            if (con != null)
            {
                con.Identifies = item.Identifies;
                con.Name = item.Name;
                con.LastKnownAddress = item.LastKnownAddress;
                con.AutoConnect = item.AutoConnect;
                return await connectionStore.UpdateItemAsync(con);
            }
            return await connectionStore.AddItemAsync(new Features.Authentication.Models.Connection
            {
                Id = Guid.NewGuid(),
                AutoConnect = item.AutoConnect,
                LastKnownAddress = item.LastKnownAddress,
                Identifies = item.Identifies,
                Name = item.Name
            });
        }

        public async Task<bool> DeleteItemAsync(Guid id)
        {
            return await connectionStore.DeleteItemAsync(id);
        }

        public async Task<DesktopConfigurationModel> GetItemAsync(Guid id)
        {
            var con = await connectionStore.GetItemAsync(id);
            return new DesktopConfigurationModel
            {
                Id = con.Id,
                AutoConnect = con.AutoConnect,
                ConnectionState = con.ConnectionState,
                Identifies = con.Identifies,
                LastConnection = con.LastConnection,
                LastKnownAddress = con.LastKnownAddress,
                Name = con.Name
            };
        }

        public async Task<IEnumerable<DesktopConfigurationModel>> GetItemsAsync(bool forceRefresh = false)
        {
            return from x in await connectionStore.GetItemsAsync()
                   select new DesktopConfigurationModel
                   {
                       Id = x.Id,
                       AutoConnect = x.AutoConnect,
                       ConnectionState = x.ConnectionState,
                       Identifies = x.Identifies,
                       LastConnection = x.LastConnection,
                       LastKnownAddress = x.LastKnownAddress,
                       Name = x.Name
                   };
        }

        public async Task<bool> UpdateItemAsync(DesktopConfigurationModel item)
        {
            var con = await connectionStore.GetItemAsync(item.Id);
            if (con != null)
            {
                con.Identifies = item.Identifies;
                con.Name = item.Name;
                con.LastKnownAddress = item.LastKnownAddress;
                con.AutoConnect = item.AutoConnect;
                return await connectionStore.UpdateItemAsync(con);
            }
            return await Task.FromResult(false);
        }
    }
}
