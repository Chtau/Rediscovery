using Rediscovery.Features.DesktopConfiguration;
using Rediscovery.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Features.Connection.ConnectService))]
namespace Rediscovery.Features.Connection
{
    public class ConnectService : BaseService, IConnectService
    {
        private CommunicationClientConsumer.IHub communicationHub => DependencyService.Get<CommunicationClientConsumer.IHub>() ?? new CommunicationClientConsumer.Hub();

        public void AutoConnect(Action<SharedCoreModels.Enums.ConnectionState> resultCallback)
        {
            throw new NotImplementedException();
        }

        public void Connect(DesktopConfigurationModel desktopConfigurationModel, Action<SharedCoreModels.Enums.ConnectionState> resultCallback)
        {
            throw new NotImplementedException();
        }
    }
}
