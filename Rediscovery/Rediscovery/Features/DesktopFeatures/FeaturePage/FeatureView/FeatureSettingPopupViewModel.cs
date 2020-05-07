using PluginFeature.Models;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.FeatureView
{
    public class FeatureSettingPopupViewModel : BaseViewModel
    {
        public event EventHandler<DeviceFeatureProfil> ProfileChanged;

        public ObservableCollection<DeviceFeatureProfil> Profiles { get; set; } = new ObservableCollection<DeviceFeatureProfil>();

        private DeviceFeatureProfil selectedProfile;

        public DeviceFeatureProfil SelectedProfile
        {
            get { return selectedProfile; }
            set
            {
                SetProperty(ref selectedProfile, value);
                ProfileChanged?.Invoke(this, value);
            }
        }
    }
}
