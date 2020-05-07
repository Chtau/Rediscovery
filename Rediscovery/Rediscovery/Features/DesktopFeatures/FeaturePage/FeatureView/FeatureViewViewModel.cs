using PluginFeature.Models;
using Rediscovery.Models;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.FeatureView
{
    public class FeatureViewViewModel : BaseFeatureViewModel
    {
        private DesktopFeatures.IFeatureUIService featureUIService => DependencyService.Get<DesktopFeatures.IFeatureUIService>() ?? new DesktopFeatures.FeatureUIService();

        public event EventHandler<Tuple<Guid, string>> UIDataReady;
        public event EventHandler<Tuple<Guid, Guid>> UIDataNoArchive;
        public event EventHandler<object> ReceivedProfilData;

        public readonly Features.Connection.Models.ConnectionManifestFeature ConnectionManifestFeature;
        public readonly Guid DesktopConfigId;

        public ObservableCollection<DeviceFeatureProfil> Profiles { get; set; } = new ObservableCollection<DeviceFeatureProfil>();

        private DeviceFeatureProfil selectedProfile;

        public DeviceFeatureProfil SelectedProfile
        {
            get { return selectedProfile; }
            set
            {
                SetProperty(ref selectedProfile, value);
                OnProfileChanged();
            }
        }

        private string currentTitle;

        public string CurrentTitle
        {
            get { return currentTitle; }
            set { SetProperty(ref currentTitle, value); }
        }

        private DeviceFeatureSetting featureSetting;

        public DeviceFeatureSetting FeatureSetting
        {
            get { return featureSetting; }
            set { SetProperty(ref featureSetting, value); }
        }

        public LoadBinding Load { get; set; }

        public FeatureViewViewModel(Guid desktopConfigId, Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature): base(connectionManifestFeature)
        {
            Load = new LoadBinding
            {
                IsLoading = false
            };
            ConnectionManifestFeature = connectionManifestFeature;
            Title = connectionManifestFeature.FeatureDisplayName;
            DesktopConfigId = desktopConfigId;
            featureUIService.SaveUI(DesktopConfigId, ConnectionManifestFeature.FeatureId, (state, directory) =>
            {
                if (state)
                {
                    UIDataReady?.Invoke(this, new Tuple<Guid, string>(ConnectionManifestFeature.FeatureId, directory));
                } else
                {
                    UIDataNoArchive?.Invoke(this, new Tuple<Guid, Guid>(DesktopConfigId, ConnectionManifestFeature.FeatureId));
                }
            });
            featureUIService.GetProfil(DesktopConfigId, ConnectionManifestFeature.FeatureId, (result, profiles) =>
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
            });
            featureUIService.GetSetting(DesktopConfigId, ConnectionManifestFeature.FeatureId, (result, settings) =>
            {
                if (result)
                {
                    FeatureSetting = settings;
                }
            });
            base.ReceivedData += FeatureViewViewModel_ReceivedData;
        }

        private void FeatureViewViewModel_ReceivedData(object sender, Tuple<string, object> e)
        {
            if (SelectedProfile == null || SelectedProfile.Id == e.Item1)
                ReceivedProfilData?.Invoke(this, e.Item2);
        }

        public void Send(object sendModel)
        {
            base.Send(SelectedProfile?.Id, sendModel);
        }

        private void OnProfileChanged()
        {
            CurrentTitle = "";
            Title = SelectedProfile.DisplayName;
        }
    }
}
