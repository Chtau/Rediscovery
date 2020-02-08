using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.TerminalPage
{
    public class TerminalFeatureViewModel : BaseFeatureViewModel
    {
        public event EventHandler<string> LineReceived;

        public TerminalFeatureViewModel(Authentication.Models.ConnectionManifestFeature connectionManifestFeature) : base(connectionManifestFeature)
        {
            base.ReceivedData += TerminalFeatureViewModel_ReceivedData;
        }

        private void TerminalFeatureViewModel_ReceivedData(object sender, object e)
        {
            string lineReceived = e.ToString();// Newtonsoft.Json.JsonConvert.DeserializeObject<string>(e.ToString());
            LineReceived?.Invoke(this, lineReceived);
        }
    }
}
