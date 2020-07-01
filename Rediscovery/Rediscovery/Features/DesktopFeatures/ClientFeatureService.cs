using Rediscovery.Features.Connection;
using Rediscovery.Features.DesktopFeatures.Models;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.DesktopFeatures.ClientFeatureService))]
namespace Rediscovery.Features.DesktopFeatures
{
    public class ClientFeatureService : BaseService, IClientFeatureService
    {
        private IManifestFeatureEntityManager entityManager => DependencyService.Get<IManifestFeatureEntityManager>() ?? new ManifestFeatureEntityManager();

        public event EventHandler<IEnumerable<Connection.Models.ConnectionManifestFeature>> OpenFeatureSelectDialog;

        public void OpenWithIntentReceived(IntentReceivedModel intentReceivedModel)
        {
            // TODO: handle if we are not connected to any device
            // TODO: if we are not connected with a device but this intent in a queue to allow the user to connect and then proceed with the action
            // TODO: check if the feature has resource setting (resource setting object should be stored in a generic serialized object in the manifest)
            var featuresForOpenWith = entityManager.ConnectionManifestFeatures?.Where(x => x.FeatureFeatureIntegrationPoint == SharedBase.Device.IntegrationPoint.Mobile && x.FeatureNativeResource.HasFlag(SharedBase.Enums.ClientNativeResources.OpenWithIntent));
            if (featuresForOpenWith?.Count() > 0)
            {
                OpenFeatureSelectDialog?.Invoke(this, featuresForOpenWith);
            } else
            {
                _logger.LogWarning("No Feature found which supports [OpenWithIntent]");
            }
        }
    }
}
