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

            Connect = new Command(async () =>
            {
                try
                {
                    Load.IsLoading = true;
                    await auth.TryConnect(Item);
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

        private void Auth_ManifestReceived(object sender, Tuple<DesktopConfiguration.DesktopConfigurationModel, List<Features.Connection.Models.ConnectionManifestFeature>> e)
        {
            //throw new NotImplementedException();
        }

        private void Auth_HelloReceived(object sender, DesktopConfiguration.DesktopConfigurationModel e)
        {
            try
            {
                Item.ConnectionState = e.ConnectionState;
                Item.LastConnection = e.LastConnection;
            } catch (Exception ex)
            {
                logger.Error(ex);
            }
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
