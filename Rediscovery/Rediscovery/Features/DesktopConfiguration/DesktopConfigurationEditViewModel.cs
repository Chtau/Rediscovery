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

        bool canEdit = false;
        public bool CanEdit
        {
            get { return canEdit; }
            set { SetProperty(ref canEdit, value); }
        }

        public Command Connect { get; }
        public Command Disconnect { get; }
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
                CanEdit = true;
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
                    Address = "192.168.1.100",
                    Port = 44341,
                    AutoConnect = true,
                    ConnectionState = SharedBase.Connection.Enums.ConnectionState.None,
                    LastConnection = null
                };
                CanEdit = true;
            }

            Connect = new Command(() =>
            {
                try
                {
                    CanEdit = false;
                    Load.IsLoading = true;
                    connectService.Connect(Item, (result, state) =>
                    {
                        try
                        {
                            Item.ConnectionState = state;
                            if (state == SharedBase.Connection.Enums.ConnectionState.OK)
                            {
                                Item.LastConnection = DateTime.Now;
                                _userNotification.ShowToast("Successful connected");
                            } else
                            {
                                _userNotification.ShowToast("Not connected");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex);
                        } finally
                        {
                            Load.IsLoading = false;
                            CanEdit = true;
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                    Load.IsLoading = false;
                }
            }, () =>
            {
                return IsConnectEnabled;
            });
            Disconnect = new Command(() =>
            {
                try
                {
                    CanEdit = false;
                    Load.IsLoading = true;
                    connectService.Disconnect(Item, (result) =>
                    {
                        try
                        {
                            Item.ConnectionState = SharedBase.Connection.Enums.ConnectionState.None;
                            _userNotification.ShowToast("Disconnected");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex);
                        }
                        finally
                        {
                            Load.IsLoading = false;
                            CanEdit = true;
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                    Load.IsLoading = false;
                }
            }, () =>
            {
                return IsConnectEnabled;
            });
            Connect.ChangeCanExecute();
            Disconnect.ChangeCanExecute();
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
                    Disconnect.ChangeCanExecute();
                    Item = result.Item2;
                    MessagingCenter.Send(this, "refresh_desktop_configuration", Item);
                    _userNotification.ShowToast("Successful saved");
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex);
                _userNotification.ShowToast("Failed to save");
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
                    _userNotification.ShowToast("Configuration deleted");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                _userNotification.ShowToast("Failed to delete");
            }
        }
    }
}
