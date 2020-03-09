using Rediscovery.Features.Authentication;
using Rediscovery.Features.Connection;
using Rediscovery.Models;
using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rediscovery.Features.DesktopConfiguration
{
    public class DesktopConfigurationEditViewModel : BaseViewModel
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDataStoreGuid<DesktopConfigurationModel> Store => DependencyService.Get<IDataStoreGuid<DesktopConfigurationModel>>() ?? new DesktopConfigurationStore();
        private IDataStoreGuid<Features.Connection.Models.ConnectionInfo> connectionStore => DependencyService.Get<IDataStoreGuid<Features.Connection.Models.ConnectionInfo>>() ?? new ConnectionStore();
        private IConnect auth => DependencyService.Get<IConnect>() ?? new Connect();

        public DesktopConfigurationModel Item { get; set; }
        public Command Connect { get; }
        public LoadBinding Load { get; set; }

        public DesktopConfigurationEditViewModel(DesktopConfigurationModel item = null)
        {
            auth.HelloReceived += Auth_HelloReceived;
            auth.ManifestReceived += Auth_ManifestReceived;
            Load = new LoadBinding
            {
                IsLoading = false
            };

            if (item != null)
            {
                Title = "Edit Device";
                Item = item;
            }
            else
            {
                Title = "New Device";
                Item = new DesktopConfigurationModel
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "Device Name",
                    LastKnownAddress = "192.168.1.160:44341",
                    User = "dev5",
                    AutoConnect = true,
                    ConnectionState = SharedCoreModels.Enums.ConnectionState.None,
                    LastConnection = null
                };
            }

            Connect = new Command(async () =>
            {
                Load.IsLoading = true;

                await auth.TryConnect(Item.Id);

                Load.IsLoading = false;
            });
        }

        private void Auth_ManifestReceived(object sender, Tuple<Features.Connection.Models.ConnectionInfo, List<Features.Connection.Models.ConnectionManifestFeature>> e)
        {
            //throw new NotImplementedException();
        }

        private void Auth_HelloReceived(object sender, Features.Connection.Models.ConnectionInfo e)
        {
            Item.ConnectionState = e.ConnectionState;
            Item.LastConnection = e.LastConnection;
        }


        public async Task Save()
        {
            await connectionStore.AddItemAsync(new Features.Connection.Models.ConnectionInfo
            {
                AutoConnect = Item.AutoConnect,
                ConnectionState = SharedCoreModels.Enums.ConnectionState.None,
                Id = Item.Id,
                User = Item.User,
                LastConnection = Item.LastConnection,
                LastKnownAddress = Item.LastKnownAddress,
                DisplayName = Item.DisplayName
            });
            MessagingCenter.Send(this, "refresh_desktop_configuration", Item);
        }

        public async Task Remove()
        {
            await connectionStore.DeleteItemAsync(Item.Id);
            MessagingCenter.Send(this, "refresh_desktop_configuration", Item);
        }
    }
}
