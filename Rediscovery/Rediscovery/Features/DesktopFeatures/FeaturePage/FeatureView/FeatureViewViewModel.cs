using PluginFeature.Models;
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
        private bool processIsRunning = false;

        private DesktopFeatures.IFeatureUIService featureUIService => DependencyService.Get<DesktopFeatures.IFeatureUIService>() ?? new DesktopFeatures.FeatureUIService();

        public event EventHandler<Tuple<Guid, string>> UIDataReady;

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

        public FeatureViewViewModel(Guid desktopConfigId, Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature): base(connectionManifestFeature)
        {
            ConnectionManifestFeature = connectionManifestFeature;
            DesktopConfigId = desktopConfigId;
            featureUIService.SaveUI(DesktopConfigId, ConnectionManifestFeature.FeatureId, (state, directory) =>
            {
                if (state)
                {
                    UIDataReady?.Invoke(this, new Tuple<Guid, string>(ConnectionManifestFeature.FeatureId, directory));
                }
            });
            base.ReceivedData += FeatureViewViewModel_ReceivedData;
            if (connectionManifestFeature.Profiles?.Count > 0)
            {
                foreach (var item in connectionManifestFeature.Profiles)
                {
                    Profiles.Add(item);
                }
                SelectedProfile = connectionManifestFeature.Profiles.First();
                OnProfileChanged();
            }
            else
            {
                Title = connectionManifestFeature.FeatureDisplayName;
            }
        }

        public void Send(object sendModel)
        {
            base.Send(SelectedProfile?.Id, sendModel);
        }

        private void FeatureViewViewModel_ReceivedData(object sender, object e)
        {
            /*MediaPlayerStateData stateData = Newtonsoft.Json.JsonConvert.DeserializeObject<MediaPlayerStateData>(e?.ToString());
            if (stateData != null && SelectedProfile != null && string.Equals(SelectedProfile.Id, stateData.ProfileId, StringComparison.OrdinalIgnoreCase))
            {
                if (processIsRunning != stateData.ProcessRunning)
                {
                    processIsRunning = stateData.ProcessRunning;
                    OnChangeCanExecute();
                }
                CurrentTitle = stateData.CurrentTitle;
            }*/
        }

        private void OnProfileChanged()
        {
            processIsRunning = false;
            CurrentTitle = "";
            Title = SelectedProfile.DisplayName;
            if (!string.IsNullOrWhiteSpace(SelectedProfile.ProfileData?.ToString()))
            {
                /*try
                {
                    Commands = System.Text.Json.JsonSerializer.Deserialize<List<CommandTypes>>(SelectedProfile.ProfileData?.ToString());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }*/
            }
        }
    }
}
