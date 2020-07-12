using System;
using System.Collections.Generic;
using System.Text;

namespace RediscoveryManager.GUI.ViewModels
{
    public class ConnectionConfigurationViewModel : ViewModelBase
    {
        public string IPAddress { get; set; }
        public int? Port { get; set; }
        public string DeviceIdentifier { get; set; }
    }
}
