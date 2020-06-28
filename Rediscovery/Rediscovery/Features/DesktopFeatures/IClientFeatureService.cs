using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures
{
    public interface IClientFeatureService
    {
        void OpenWithIntentReceived(Features.DesktopFeatures.Models.IntentReceivedModel intentReceivedModel);
    }
}
