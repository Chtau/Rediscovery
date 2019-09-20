using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Rediscovery.DesktopConfiguration;
using SharedCoreModels;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Authentication.Connect))]
namespace Rediscovery.Features.Authentication
{
    public class Connect : IConnect
    {
        private HubConnection connection;

        public event EventHandler<DesktopConfigurationModel> HelloReceived;
        public event EventHandler<Tuple<DesktopConfigurationModel, Manifest>> ManifestReceived;

        public Connect()
        {

        }

        private void OnHello(HubConnection con, DesktopConfigurationModel model)
        {
            con.On<bool, string>("Hello", (status, info) =>
            {
                model.ConnectionState = DesktopConfigurationModel.Connection.OK;
                model.LastConnection = DateTime.Now;
                HelloReceived.Invoke(this, model);
            });
        }

        private void OnManifest(HubConnection con, DesktopConfigurationModel model)
        {
            con.On<Manifest>("Manifest", (manifest) =>
            {
                ManifestReceived?.Invoke(this, new Tuple<DesktopConfigurationModel, Manifest>(model, manifest));
            });
        }

        public async Task TryConnect(DesktopConfigurationModel model)
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://" + model.LastKnownAddress + "/connect")
                .Build();
            await connection.StartAsync();
            OnHello(connection, model);
            OnManifest(connection, model);
            await connection.InvokeAsync("Welcome", "dev1", model.Id);
        }
    }
}
