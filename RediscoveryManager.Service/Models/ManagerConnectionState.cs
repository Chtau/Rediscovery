using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.Service.Models
{
    public class ManagerConnectionState
    {
        public SharedBase.Connection.Enums.AllowConnect CanConnect { get; set; } = SharedBase.Connection.Enums.AllowConnect.None;
        public SharedBase.Connection.Enums.ConnectionState ConnectionState { get; set; } = SharedBase.Connection.Enums.ConnectionState.None;
    }
}
