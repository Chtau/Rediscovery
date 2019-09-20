using Rediscovery.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.Authentication.Models
{
    public class Connection : BaseModel
    {
        private Guid _id;
        private string _name;
        private string _identifies;
        private string _lastKnownAddress;
        private DateTime? _lastConnection = null;
        private bool _autoConnect;
        private SharedCoreModels.Enums.ConnectionState _connectionState;
        private string _manifestClientName;
        private string _manifestClientVersion;
        private string _manifestAppMinimumVersion;

        [PrimaryKey]
        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Identifies
        {
            get { return _identifies; }
            set { SetProperty(ref _identifies, value); }
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

        public SharedCoreModels.Enums.ConnectionState ConnectionState
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
