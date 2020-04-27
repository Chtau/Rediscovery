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
        private IEntityManager entityManager => DependencyService.Get<IEntityManager>() ?? new EntityManager();
        public ObservableCollection<Features.Connection.Models.ConnectionManifestFeature> ConnectionManifestFeaturesControl { get; set; } = new ObservableCollection<Features.Connection.Models.ConnectionManifestFeature>();

        public DesktopFeaturesViewModel()
        {
            if (ConnectionManifestFeaturesControl == null)
                ConnectionManifestFeaturesControl = new ObservableCollection<Features.Connection.Models.ConnectionManifestFeature>();
            // TODO: show for every feature from all connected desktops
            var items = entityManager.GetConnectedConnectionManifestFeature();
            if (items != null)
            {
                foreach (var item in items)
                {
                    ConnectionManifestFeaturesControl.Add(item);
                }
            }
            entityManager.ConnectionManifestFeatures.CollectionChanged += ConnectionManifestFeatures_CollectionChanged;
        }

        private void ConnectionManifestFeatures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Features.Connection.Models.ConnectionManifestFeature item in e.NewItems)
            {
                ConnectionManifestFeaturesControl.Add(item);
            }
        }
    }
}
