using Rediscovery.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.Authentication.Models
{
    public class ConnectionManifestFeature : BaseModel
    {
        private Guid _connectionId;
        private string _featureKey;

        [PrimaryKey]
        public Guid ConnectionId
        {
            get { return _connectionId; }
            set { SetProperty(ref _connectionId, value); }
        }

        public string FeatureKey
        {
            get { return _featureKey; }
            set { SetProperty(ref _featureKey, value); }
        }
    }
}
