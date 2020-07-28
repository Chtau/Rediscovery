using Rediscovery.Features.Connection;
using Rediscovery.Features.DesktopConfiguration;
using Rediscovery.Models;
using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Rediscovery.Features.Startpage
{
    public class StartViewModel : BaseViewModel
    {
        public IDataStoreGuid<DesktopConfigurationModel> desktopConfigStore => DependencyService.Get<IDataStoreGuid<DesktopConfigurationModel>>() ?? new DesktopConfigurationStore();
        private IConnectService connectService => DependencyService.Get<IConnectService>() ?? new ConnectService();
        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();

        public Command OpenUrlCommand { get; set; }
        public Command QuickConnectCommand { get; set; }
        public Command QuickFeatureCommand { get; set; }

        public LoadBinding Load { get; set; }

        public event EventHandler<Features.Connection.Models.ConnectionManifestFeature> QuickFeatureSelected;

        public StartViewModel()
        {
            entityManager.ConnectionManifestFeatures.CollectionChanged += ConnectionManifestFeatures_CollectionChanged;
            Load = new LoadBinding
            {
                IsLoading = false
            };

            OpenUrlCommand = new Command<string>(async (url) => await Launcher.OpenAsync(url));
            QuickConnectCommand = new Command(() =>
            {
                Load.IsLoading = true;
                QuickConnectCommand.ChangeCanExecute();
                Task.Run(() =>
                {
                    try
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
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                    finally
                    {
                        Load.IsLoading = false;
                        QuickConnectCommand.ChangeCanExecute();
                    }
                });
            }, () =>
            {
                return !Load.IsLoading;
            });
            QuickFeatureCommand = new Command<Features.Connection.Models.ConnectionManifestFeature>((feature) =>
            {
                if (feature != null)
                {
                    if (feature.FeatureFeatureIntegrationPoint == SharedBase.Device.IntegrationPoint.Desktop)
                    {
                        QuickFeatureSelected?.Invoke(this, feature);
                    }
                }
            });
            connectService.HeartbeatStateChanges += ConnectService_HeartbeatStateChanges;
        }

        private void ConnectionManifestFeatures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.OldItems != null)
                {
                    foreach (Features.Connection.Models.ConnectionManifestFeature item in e.OldItems)
                    {
                        if (ConnectionManifestFeaturesControl.Contains(item))
                            ConnectionManifestFeaturesControl.Remove(item);
                    }
                }
                if (e.NewItems != null)
                {
                    foreach (Features.Connection.Models.ConnectionManifestFeature item in e.NewItems)
                    {
                        if (item.FeatureFeatureIntegrationPoint == SharedBase.Device.IntegrationPoint.Desktop)
                            ConnectionManifestFeaturesControl.Add(item);
                    }
                }
                while (ConnectionManifestFeaturesControl.Count > 2)
                {
                    ConnectionManifestFeaturesControl.RemoveAt(0);
                }
                if (ConnectionManifestFeaturesControl.Count > 0 && IsConnect)
                {
                    ShowQuickFeatures = true;
                } else
                {
                    ShowQuickFeatures = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
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

        private DesktopConfigurationModel desktopConfigurationModel;
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

        private bool showQuickFeatures;
        public bool ShowQuickFeatures
        {
            get { return showQuickFeatures; }
            set { SetProperty(ref showQuickFeatures, value); }
        }

        public ObservableCollection<Features.Connection.Models.ConnectionManifestFeature> ConnectionManifestFeaturesControl { get; set; } = new ObservableCollection<Features.Connection.Models.ConnectionManifestFeature>();
    }
}
