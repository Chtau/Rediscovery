using Rediscovery.Features.Authentication;
using Rediscovery.Features.Authentication.Models;
using Rediscovery.Features.Connection;
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
        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();
        public ObservableCollection<Features.Connection.Models.ConnectionManifestFeature> ConnectionManifestFeaturesControl { get; set; } = new ObservableCollection<Features.Connection.Models.ConnectionManifestFeature>();

        private string filterIcon = "filter_empty.png";

        public string FilterIcon
        {
            get { return filterIcon; }
            set { SetProperty(ref filterIcon, value); }
        }

        public DesktopFeaturesViewModel()
        {
            if (ConnectionManifestFeaturesControl == null)
                ConnectionManifestFeaturesControl = new ObservableCollection<Features.Connection.Models.ConnectionManifestFeature>();
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
            try
            {
                if (e.OldItems != null)
                {
                    foreach (Features.Connection.Models.ConnectionManifestFeature item in e.OldItems)
                    {
                        if (ConnectionManifestFeaturesControl.Contains(item))
                            ConnectionManifestFeaturesControl.Remove(item);
                    }
                }
                if (e.NewItems != null)
                {
                    foreach (Features.Connection.Models.ConnectionManifestFeature item in e.NewItems)
                    {
                        ConnectionManifestFeaturesControl.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }
    }
}
