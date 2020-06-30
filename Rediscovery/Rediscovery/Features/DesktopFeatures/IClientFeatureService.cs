using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IClientFeatureService
    {
        event EventHandler<IEnumerable<Connection.Models.ConnectionManifestFeature>> OpenFeatureSelectDialog;
        void OpenWithIntentReceived(Features.DesktopFeatures.Models.IntentReceivedModel intentReceivedModel);
    }
}
