using Rediscovery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.Settings.Models
{
    public class SettingModel : BaseModel
    {
        private Guid _id;
        private string _deviceIdentifier;
        private int _discoveryPort;
        private int _connectTimeout;

        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string DeviceName
        {
            get { return _deviceIdentifier; }
            set { SetProperty(ref _deviceIdentifier, value); }
        }

        public int DiscoveryPort
        {
            get { return _discoveryPort; }
            set { SetProperty(ref _discoveryPort, value); }
        }

        public int ConnectTimeout
        {
            get { return _connectTimeout; }
            set { SetProperty(ref _connectTimeout, value); }
        }

        public SettingModel()
        {
            Id = Guid.Empty;
            DiscoveryPort = 8888;
            DeviceName = Xamarin.Essentials.DeviceInfo.Name;
            ConnectTimeout = 2;
        }
    }
}
