using Rediscovery.Features.Authentication;
using Rediscovery.Features.Authentication.Models;
using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rediscovery.Features.DesktopFeatures
{
    public class DesktopFeaturesViewModel : BaseViewModel
    {
        private IDataStoreConnectionGuid<ConnectionManifestFeature> connectionManifestFeatureStore => DependencyService.Get<IDataStoreConnectionGuid<ConnectionManifestFeature>>() ?? new ConnectionManifestFeatureStore();
        public ObservableCollection<Features.Authentication.Models.ConnectionManifestFeature> ConnectionManifestFeaturesControl { get; set; } = new ObservableCollection<Features.Authentication.Models.ConnectionManifestFeature>();

        public async Task Load()
        {
            if (ConnectionManifestFeaturesControl == null)
                ConnectionManifestFeaturesControl = new ObservableCollection<ConnectionManifestFeature>();
            ConnectionManifestFeaturesControl.Clear();
            var items = await connectionManifestFeatureStore.GetItemsAsync();
            if (items != null)
            {
                foreach (var item in items)
                {
                    ConnectionManifestFeaturesControl.Add(item);
                }
            }
        }
    }
}
