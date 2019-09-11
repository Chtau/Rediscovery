using Microsoft.AspNetCore.SignalR.Client;
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

        public DesktopConfigurationModel Item { get; set; }
        public Command Connect { get; }
        public LoadBinding Load { get; set; }

        public DesktopConfigurationEditViewModel(DesktopConfigurationModel item = null)
        {
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
                    Name = "Device Name",
                    LastKnownAddress = "192.168.1.160:44314",
                    AutoConnect = true,
                    ConnectionState = Connection.None,
                    LastConnection = null
                };
            }

            HubConnection connection;
            Connect = new Command(async () =>
            {
                Load.IsLoading = true;

                connection = new HubConnectionBuilder()
                .WithUrl("http://" + Item.LastKnownAddress + "/connect")
                .Build();
                await connection.StartAsync();
                connection.On<bool>("Hello", (status) =>
                {
                    Item.ConnectionState = Connection.OK;
                    item.LastConnection = DateTime.Now;
                });
                await connection.InvokeAsync("Welcome", "dev1", Item.Id);
                

                Load.IsLoading = false;
            });
        }

        public async Task Save()
        {
            await Store.AddItemAsync(Item);
            MessagingCenter.Send(this, "refresh_desktop_configuration", Item);
        }

        public async Task Remove()
        {
            await Store.DeleteItemAsync(Item.Id);
            MessagingCenter.Send(this, "refresh_desktop_configuration", Item);
        }
    }
}
