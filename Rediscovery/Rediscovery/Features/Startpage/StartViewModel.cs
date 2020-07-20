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

        public StartViewModel()
        {
            OpenUrlCommand = new Command<string>(async (url) =>
            {
                await Launcher.OpenAsync(url);
            });
        }

        public void UpdateGetQuickConnectItem()
        {
            try
            {
                var items = desktopConfigStore.GetItems();
                if (items?.Count() == 1)
                    DesktopConfiguration = items.First();
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        // TODO: Show quick connect menu (last connected or near know network or if we have only one configuration)
        // if we are connect show to which configuration we are connected
        // if we have no configuration inform the user that we must add a configuration

        bool isConnect;
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
    }
}
