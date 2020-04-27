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
        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();
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
                // TODO: change default Values for new connection
                Title = "New Device";
                Item = new DesktopConfigurationModel
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "New",
                    LastKnownAddress = "192.168.1.100:44341",
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

        private void Auth_ManifestReceived(object sender, Tuple<DesktopConfiguration.DesktopConfigurationModel, List<Features.Connection.Models.ConnectionManifestFeature>> e)
        {
            //throw new NotImplementedException();
        }

        private void Auth_HelloReceived(object sender, DesktopConfiguration.DesktopConfigurationModel e)
        {
            Item.ConnectionState = e.ConnectionState;
            Item.LastConnection = e.LastConnection;
        }


        public async Task Save()
        {
            await desktopStore.AddItemAsync(new DesktopConfiguration.DesktopConfigurationModel
            {
                AutoConnect = Item.AutoConnect,
                ConnectionState = SharedCoreModels.Enums.ConnectionState.None,
                Id = Item.Id,
                LastConnection = Item.LastConnection,
                LastKnownAddress = Item.LastKnownAddress,
                DisplayName = Item.DisplayName
            });
            MessagingCenter.Send(this, "refresh_desktop_configuration", Item);
        }

        public async Task Remove()
        {
            await desktopStore.DeleteItemAsync(Item.Id);
            MessagingCenter.Send(this, "refresh_desktop_configuration", Item);
        }
    }
}
