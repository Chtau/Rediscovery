using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.Models
{
    public class ManagerConnectionState
    {
        public Shared.Base.Connection.Enums.AllowConnect CanConnect { get; set; } = Shared.Base.Connection.Enums.AllowConnect.None;
        public Shared.Base.Connection.Enums.ConnectionState ConnectionState { get; set; } = Shared.Base.Connection.Enums.ConnectionState.None;
    }
}
