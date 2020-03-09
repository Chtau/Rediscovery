using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.Connection
{
    public class FeaturesConnectionHub : InternalHubs, IInternalHub
    {
        const string hubLink = "/hubs/feature";

        public FeaturesConnectionHub() : base(hubLink)
        {

        }

        public async Task<HubConnection> GetConnection(Models.ConnectionInfo model)
        {
            return await base.OnGetConnection(model, true);
        }
    }
}
