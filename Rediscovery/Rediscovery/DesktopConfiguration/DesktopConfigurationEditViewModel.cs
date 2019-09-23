using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.Features.Authentication;
using Rediscovery.Models;
using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Rediscovery.DesktopConfiguration.DesktopConfigurationModel;

namespace Rediscovery.DesktopConfiguration
{
    public class DesktopConfigurationEditViewModel : BaseViewModel
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDataStoreGuid<DesktopConfigurationModel> Store => DependencyService.Get<IDataStoreGuid<DesktopConfigurationModel>>() ?? new DesktopConfigurationStore();
        private IDataStoreGuid<Features.Authentication.Models.Connection> connectionStore => DependencyService.Get<IDataStoreGuid<Features.Authentication.Models.Connection>>() ?? new ConnectionStore();
        private Features.Authentication.IConnect auth => DependencyService.Get<Features.Authentication.IConnect>() ?? new Features.Authentication.Connect();

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
                    LastKnownAddress = "192.168.1.160:44314",
                    AutoConnect = true,
                    ConnectionState = SharedCoreModels.Enums.ConnectionState.None,
                    LastConnection = null
                };
            }

            //HubConnection connection;
            Connect = new Command(async () =>
            {
                Load.IsLoading = true;

                await auth.TryConnect(Item.Id);
                /*connection = new HubConnectionBuilder()
                .WithUrl("http://" + Item.LastKnownAddress + "/connect")
                .Build();
                await connection.StartAsync();
                connection.On<bool, string>("Hello", (status, info) =>
                {
                    Item.ConnectionState = Connection.OK;
                    item.LastConnection = DateTime.Now;
                });
                await connection.InvokeAsync("Welcome", "dev1", Item.Id);
                */

                Load.IsLoading = false;
            });
        }

        private void Auth_ManifestReceived(object sender, Tuple<Features.Authentication.Models.Connection, List<Features.Authentication.Models.ConnectionManifestFeature>> e)
        {
            //throw new NotImplementedException();
        }

        private void Auth_HelloReceived(object sender, Features.Authentication.Models.Connection e)
        {
            Item.ConnectionState = e.ConnectionState;
            Item.LastConnection = e.LastConnection;
        }


        public async Task Save()
        {
            await connectionStore.AddItemAsync(new Features.Authentication.Models.Connection
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
