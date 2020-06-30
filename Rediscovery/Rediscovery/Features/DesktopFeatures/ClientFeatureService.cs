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
            var featuresForOpenWith = entityManager.ConnectionManifestFeatures?.Where(x => x.FeatureFeatureIntegrationPoint == SharedBase.Device.IntegrationPoint.Mobile && x.FeatureNativeResource.HasFlag(SharedBase.Enums.ClientNativeResources.OpenWithIntent));
            if (featuresForOpenWith?.Count() > 0)
            {
                
            } else
            {
                _logger.LogWarning("No Feature found which supports [OpenWithIntent]");
            }
        }
    }
}
