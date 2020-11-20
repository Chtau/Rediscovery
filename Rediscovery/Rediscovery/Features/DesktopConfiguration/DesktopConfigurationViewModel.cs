using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using Rediscovery.Features.Connection;
using Rediscovery.Features.Storage;

namespace Rediscovery.Features.DesktopConfiguration
{
    public class DesktopConfigurationViewModel : BaseViewModel
    {
        public IDataStoreGuid<DesktopConfigurationModel> Store => DependencyService.Get<IDataStoreGuid<DesktopConfigurationModel>>() ?? new DesktopConfigurationStore();

        public ObservableCollection<DesktopConfigurationModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        private IConnectService connectService => DependencyService.Get<IConnectService>() ?? new ConnectService();

        public DesktopConfigurationViewModel()
        {
            connectService.HeartbeatStateChanges += ConnectService_HeartbeatStateChanges;
            Title = "Desktop";
            Items = new ObservableCollection<DesktopConfigurationModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadDeviceItemsCommand());
            MessagingCenter.Subscribe<DesktopConfigurationEditViewModel, DesktopConfigurationModel>(this, "refresh_desktop_configuration", async (obj, args) =>
            {
                await ExecuteLoadDeviceItemsCommand();
            });
        }

        private void ConnectService_HeartbeatStateChanges(object sender, Guid desktopConfigurationId)
        {
            try
            {
                var round = connectService.GetHeartbeat(desktopConfigurationId);
                var item = Items.FirstOrDefault(x => x.Id == desktopConfigurationId);
                if (item != null)
                {
                    item.ConnectionState = round.OK ? SharedBase.Connection.Enums.ConnectionState.OK : SharedBase.Connection.Enums.ConnectionState.None;
                    item.PingPingTime = round.PingPongTime;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        async Task ExecuteLoadDeviceItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await Store.GetItemsAsync(true);
                if (items != null && items.Count() > 0)
                {
                    foreach (var item in items)
                    {
                        var round = connectService.GetHeartbeat(item.Id);
                        item.ConnectionState = round.OK ? SharedBase.Connection.Enums.ConnectionState.OK : SharedBase.Connection.Enums.ConnectionState.None;
                        Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
