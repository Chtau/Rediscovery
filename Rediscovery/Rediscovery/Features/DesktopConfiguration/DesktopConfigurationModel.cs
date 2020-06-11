using Rediscovery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.DesktopConfiguration
{
    public class DesktopConfigurationModel : BaseModel
    {
        private Guid _id;
        private string _displayName;
        private string _lastKnownAddress;
        private DateTime? _lastConnection = null;
        private bool _autoConnect;
        private SharedBase.Connection.Enums.ConnectionState _connectionState;
        private string _manifestClientName;
        private string _manifestClientVersion;
        private string _manifestAppMinimumVersion;

        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        public string LastKnownAddress
        {
            get { return _lastKnownAddress; }
            set { SetProperty(ref _lastKnownAddress, value); }
        }

        public DateTime? LastConnection
        {
            get { return _lastConnection; }
            set { SetProperty(ref _lastConnection, value); }
        }

        public bool AutoConnect
        {
            get { return _autoConnect; }
            set { SetProperty(ref _autoConnect, value); }
        }

        public SharedBase.Connection.Enums.ConnectionState ConnectionState
        {
            get { return _connectionState; }
            set { SetProperty(ref _connectionState, value); }
        }

        public string ManifestClientName
        {
            get { return _manifestClientName; }
            set { SetProperty(ref _manifestClientName, value); }
        }

        public string ManifestClientVersion
        {
            get { return _manifestClientVersion; }
            set { SetProperty(ref _manifestClientVersion, value); }
        }

        public string ManifestAppMinimumVersion
        {
            get { return _manifestAppMinimumVersion; }
            set { SetProperty(ref _manifestAppMinimumVersion, value); }
        }
    }
}
