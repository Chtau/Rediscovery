using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures
{
    public class DesktopFeaturesViewModel : BaseViewModel
    {
        public ObservableCollection<Features.Authentication.Models.ConnectionManifestFeature> ConnectionManifestFeaturesControl { get; set; } = new ObservableCollection<Features.Authentication.Models.ConnectionManifestFeature>();

        public void Load()
        {

        }
    }
}
