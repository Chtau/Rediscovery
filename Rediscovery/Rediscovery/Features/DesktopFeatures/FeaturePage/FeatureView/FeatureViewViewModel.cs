using Rediscovery.Features.Storage;
using Rediscovery.Models;
using Rediscovery.Services;
using Rediscovery.ViewModels;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.FeatureView
{
    public class FeatureViewViewModel : BaseViewModel//BaseFeatureViewModel
    {
        private DesktopFeatures.IFeatureService featureService => DependencyService.Get<DesktopFeatures.IFeatureService>() ?? new DesktopFeatures.FeatureService();
        private IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel> desktopStore => DependencyService.Get<IDataStoreGuid<DesktopConfiguration.DesktopConfigurationModel>>() ?? new DesktopConfiguration.DesktopConfigurationStore();

        public event EventHandler<Tuple<Guid, string>> UIDataReady;

        public event EventHandler<Tuple<Guid, Guid>> UIDataNoArchive;

        public event EventHandler<object> ReceivedFeatureData;

        public event EventHandler<object> ProfilChanged;

        public readonly Features.Connection.Models.ConnectionManifestFeature ConnectionManifestFeature;
        public readonly Guid DesktopConfigId;

        public ObservableCollection<FeatureProfil> Profiles { get; set; } = new ObservableCollection<FeatureProfil>();

        private FeatureProfil selectedProfile;

        public FeatureProfil SelectedProfile
        {
            get { return selectedProfile; }
            set
            {
                SetProperty(ref selectedProfile, value);
                OnProfileChanged();
            }
        }

        private FeatureSetting featureSetting;

        public FeatureSetting FeatureSetting
        {
            get { return featureSetting; }
            set { SetProperty(ref featureSetting, value); }
        }

        public LoadBinding Load { get; set; }

        public FeatureViewViewModel(Guid desktopConfigId, Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature)//: base(connectionManifestFeature)
        {
            Load = new LoadBinding
            {
                IsLoading = false
            };
            ConnectionManifestFeature = connectionManifestFeature;
            Title = connectionManifestFeature.FeatureDisplayName;
            DesktopConfigId = desktopConfigId;

            var config = desktopStore.GetItem(desktopConfigId);

            featureService.ReceivedData += FeatureService_ReceivedData;
            featureService.ReceivedProfiles += FeatureService_ReceivedProfiles;
            featureService.ReceivedSetting += FeatureService_ReceivedSetting;
            featureService.ReceivedUI += FeatureService_ReceivedUI;

            if (featureService.LoadFeature(config, ConnectionManifestFeature.FeatureId))
            {
            }

            /*featureUIService.SaveUI(DesktopConfigId, ConnectionManifestFeature.FeatureId, (state, directory) =>
            {
                if (state)
                {
                    UIDataReady?.Invoke(this, new Tuple<Guid, string>(ConnectionManifestFeature.FeatureId, directory));
                } else
                {
                    UIDataNoArchive?.Invoke(this, new Tuple<Guid, Guid>(DesktopConfigId, ConnectionManifestFeature.FeatureId));
                }
            });*/
            /*featureUIService.GetProfil(DesktopConfigId, ConnectionManifestFeature.FeatureId, (result, profiles) =>
            {
                if (result)
                {
                    if (profiles?.Count > 0)
                    {
                        foreach (var item in profiles)
                        {
                            Profiles.Add(item);
                        }
                        SelectedProfile = Profiles.First();
                        OnProfileChanged();
                    }
                }
            });*/
            /*featureUIService.GetSetting(DesktopConfigId, ConnectionManifestFeature.FeatureId, (result, settings) =>
            {
                if (result)
                {
                    FeatureSetting = settings;
                }
            });
            base.ReceivedData += FeatureViewViewModel_ReceivedData;*/
        }

        private void FeatureService_ReceivedUI(object sender, Tuple<bool, string> e)
        {
            if (e.Item1)
            {
                UIDataReady?.Invoke(this, new Tuple<Guid, string>(ConnectionManifestFeature.FeatureId, e.Item2));
            }
            else
            {
                UIDataNoArchive?.Invoke(this, new Tuple<Guid, Guid>(DesktopConfigId, ConnectionManifestFeature.FeatureId));
            }
        }

        private void FeatureService_ReceivedSetting(object sender, FeatureSetting e)
        {
            FeatureSetting = e;
        }

        private void FeatureService_ReceivedProfiles(object sender, List<FeatureProfil> e)
        {
            if (e?.Count > 0)
            {
                foreach (var item in e)
                {
                    Profiles.Add(item);
                }
                SelectedProfile = Profiles.First();
                OnProfileChanged();
            }
        }

        private void FeatureService_ReceivedData(object sender, FeatureData e)
        {
            if (SelectedProfile == null || string.Equals(SelectedProfile.Id, e.ProfileId, StringComparison.OrdinalIgnoreCase))
                ReceivedFeatureData?.Invoke(this, e.Data);
        }

        /*private void FeatureViewViewModel_ReceivedData(object sender, Tuple<string, object> e)
        {
            if (SelectedProfile == null || string.Equals(SelectedProfile.Id, e.Item1, StringComparison.OrdinalIgnoreCase))
                ReceivedFeatureData?.Invoke(this, e.Item2);
        }*/

        public void Send(string sendModel)
        {
            featureService.Send(SelectedProfile?.Id, sendModel);
            //base.Send(SelectedProfile?.Id, sendModel);
        }

        public void Start()
        {
            featureService.Start();
        }

        public void Stop()
        {
            featureService.Stop();
        }

        private void OnProfileChanged()
        {
            Title = SelectedProfile.DisplayName;
            ProfilChanged?.Invoke(this, SelectedProfile?.ProfileData);
        }
    }
}