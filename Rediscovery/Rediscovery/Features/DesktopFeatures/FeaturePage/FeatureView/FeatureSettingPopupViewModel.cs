using Rediscovery.ViewModels;
using SharedBase.Feature;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.FeatureView
{
    public class FeatureSettingPopupViewModel : BaseViewModel
    {
        public event EventHandler<FeatureProfil> ProfileChanged;

        public ObservableCollection<FeatureProfil> Profiles { get; set; } = new ObservableCollection<FeatureProfil>();

        private FeatureProfil selectedProfile;

        public FeatureProfil SelectedProfile
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
