using Rediscovery.Services;
using Rediscovery.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Rediscovery.Desktops
{
    public class DesktopViewModel : BaseViewModel
    {
        private IDataStoreGuid<Features.Authentication.Models.ConnectionManifestFeature> connectionManifestStore => DependencyService.Get<IDataStoreGuid<Features.Authentication.Models.ConnectionManifestFeature>>() ?? new Features.Authentication.ConnectionManifestFeatureStore();

        public Features.Authentication.Models.Connection Connection { get; private set; }
        public ObservableCollection<Features.Authentication.Models.ConnectionManifestFeature> ConnectionManifestFeatures { get; set; } = new ObservableCollection<Features.Authentication.Models.ConnectionManifestFeature>();

        public DesktopViewModel(Features.Authentication.Models.Connection con)
        {
            Connection = con;
        }

        public void Load()
        {
            Task.Run(async () =>
            {
                var items = await connectionManifestStore.GetItemsAsync();
                foreach (var item in items)
                {
                    ConnectionManifestFeatures.Add(item);
                }
            });
        }
    }
}
