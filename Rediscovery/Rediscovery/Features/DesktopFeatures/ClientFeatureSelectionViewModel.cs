using Rediscovery.Features.DesktopFeatures.Models;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures
{
    public class ClientFeatureSelectionViewModel : BaseViewModel
    {
        public ObservableCollection<Features.Connection.Models.ConnectionManifestFeature> Features { get; set; } = new ObservableCollection<Features.Connection.Models.ConnectionManifestFeature>();
        public Features.Connection.Models.ConnectionManifestFeature SelectedFeature { get; set; }

        public ClientFeatureSelectionViewModel(IEnumerable<Connection.Models.ConnectionManifestFeature> features)
        {
            Features.Clear();
            foreach (var item in features)
            {
                Features.Add(item);
            }
        }
    }
}
