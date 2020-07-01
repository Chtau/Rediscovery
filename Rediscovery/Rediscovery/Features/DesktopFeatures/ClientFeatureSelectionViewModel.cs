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
        private readonly Action<Connection.Models.ConnectionManifestFeature> _callback;

        public ObservableCollection<Features.Connection.Models.ConnectionManifestFeature> Features { get; set; } = new ObservableCollection<Features.Connection.Models.ConnectionManifestFeature>();

        public ClientFeatureSelectionViewModel(IEnumerable<Connection.Models.ConnectionManifestFeature> features, Action<Connection.Models.ConnectionManifestFeature> callback)
        {
            _callback = callback;
            Features.Clear();
            foreach (var item in features)
            {
                Features.Add(item);
            }
        }

        public void SetSelectedFeaturer(Features.Connection.Models.ConnectionManifestFeature feature)
        {
            _callback?.Invoke(feature);
        }
    }
}
