using Rediscovery.Features.Connection;
using Rediscovery.Features.DesktopConfiguration;
using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Rediscovery.Features.Startpage
{
    public class StartViewModel : BaseViewModel
    {
        public IDataStoreGuid<DesktopConfigurationModel> desktopConfigStore => DependencyService.Get<IDataStoreGuid<DesktopConfigurationModel>>() ?? new DesktopConfigurationStore();
        private IConnectService connectService => DependencyService.Get<IConnectService>() ?? new ConnectService();

        public Command OpenUrlCommand { get; set; }
        public Command QuickConnectCommand { get; set; }

        public StartViewModel()
        {
            OpenUrlCommand = new Command<string>(async (url) =>
            {
                await Launcher.OpenAsync(url);
            });
            QuickConnectCommand = new Command(() =>
            {
                connectService.Connect(desktopConfigurationModel, (result, state) =>
                {
                    if (state == SharedBase.Connection.Enums.ConnectionState.OK)
                    {
                        _userNotification.ShowToast("Successful connected");
                    }
                    else
                    {
                        _userNotification.ShowToast("Not connected");
                    }
                    UpdateGetQuickConnectItem();
                });
            });
            connectService.HeartbeatStateChanges += ConnectService_HeartbeatStateChanges;
        }

        private void ConnectService_HeartbeatStateChanges(object sender, Guid e)
        {
            UpdateGetQuickConnectItem();
        }

        public void UpdateGetQuickConnectItem()
        {
            try
            {
                var items = desktopConfigStore.GetItems();
                if (items?.Count() == 1)
                {
                    NoConfiguration = false;
                    DesktopConfiguration = items.First();
                }
                else if (items?.Count() > 1)
                {
                    NoConfiguration = false;
                    var item = items.OrderByDescending(x => x.LastConnection.Value).FirstOrDefault();
                    if (item != null)
                    {
                        DesktopConfiguration = item;
                    }
                } else
                {
                    NoConfiguration = true;
                }
                if (DesktopConfiguration != null)
                {
                    var lastHeartbeat = connectService.GetHeartbeat(DesktopConfiguration.Id);
                    IsConnect = lastHeartbeat.OK;
                    PingPongTime = lastHeartbeat.PingPongTime;
                } else
                {
                    IsConnect = false;
                    PingPongTime = null;
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private bool noConfiguration;
        public bool NoConfiguration
        {
            get { return noConfiguration; }
            set { SetProperty(ref noConfiguration, value); }
        }

        private bool isConnect;
        public bool IsConnect
        {
            get { return isConnect; }
            set { SetProperty(ref isConnect, value); }
        }

        DesktopConfigurationModel desktopConfigurationModel;
        public DesktopConfigurationModel DesktopConfiguration
        {
            get { return desktopConfigurationModel; }
            set { SetProperty(ref desktopConfigurationModel, value); }
        }

        private TimeSpan? pingPongTime;
        public TimeSpan? PingPongTime
        {
            get { return pingPongTime; }
            set { SetProperty(ref pingPongTime, value); }
        }
    }
}
