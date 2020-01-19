using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Features.Authentication
{
    public class FeaturesConnectionHub : InternalHubs, IInternalHub
    {
        const string hubLink = "/hubs/feature";

        public FeaturesConnectionHub() : base(hubLink)
        {

        }

        public async Task<HubConnection> GetConnection(Models.Connection model)
        {
            return await base.OnGetConnection(model, true);
        }
    }
}
