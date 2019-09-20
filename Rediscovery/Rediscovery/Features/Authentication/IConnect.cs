using Rediscovery.DesktopConfiguration;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.Authentication
{
    public interface IConnect
    {
        Task TryConnect(DesktopConfigurationModel model);
        event EventHandler<DesktopConfigurationModel> HelloReceived;
        event EventHandler<Tuple<DesktopConfigurationModel, Manifest>> ManifestReceived;
    }
}
