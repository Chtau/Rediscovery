using Rediscovery.Features.Connection;
using Rediscovery.Models;
using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rediscovery.Features.DesktopConfiguration
{
    public class DesktopConfigurationEditViewModel : BaseViewModel
    {
        private ILogger logger => DependencyService.Get<ILogger>() ?? new Logger();
        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();
        private IConnectService connectService => DependencyService.Get<IConnectService>() ?? new ConnectService();
        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();

        DesktopConfigurationModel item;
        public DesktopConfigurationModel Item
        {
            get { return item; }
            set { SetProperty(ref item, value); }
        }

        bool isConnectEnabled = false;
        public bool IsConnectEnabled
        {
            get { return isConnectEnabled; }
            set { SetProperty(ref isConnectEnabled, value); }
        }

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
                IsConnectEnabled = true;
            }
            else
            {
                // TODO: change default Values for new connection
                IsConnectEnabled = false;
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

            Connect = new Command(() =>
            {
                try
                {
                    Load.IsLoading = true;
                    connectService.Connect(Item, (result) =>
                    {
                        try
                        {
                            Item.ConnectionState = result;
                            Item.LastConnection = DateTime.Now;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                } finally
                {
                    Load.IsLoading = false;
                }
            }, () =>
            {
                return IsConnectEnabled;
            });
            Connect.ChangeCanExecute();
        }

        public async Task Save()
        {
            try
            {
                var result = await desktopStore.AddItemAsync(Item);
                if (result.Item1)
                {
                    IsConnectEnabled = true;
                    Connect.ChangeCanExecute();
                    Item = result.Item2;
                    MessagingCenter.Send(this, "refresh_desktop_configuration", Item);
                }
            } catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public async Task Remove()
        {
            try
            {
                var result = await desktopStore.DeleteItemAsync(Item.Id);
                if (result)
                {
                    entityManager.Clear(Item.Id);
                    MessagingCenter.Send(this, "refresh_desktop_configuration", Item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
