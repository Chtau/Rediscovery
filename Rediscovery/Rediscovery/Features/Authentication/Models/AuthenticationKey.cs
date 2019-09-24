using Rediscovery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Features.Authentication.Models
{
    public class AuthenticationKey : BaseModel
    {
        private string _key;
        private Guid _connectionId;
        private SharedCoreModels.Enums.ConnectionState _connectionState = SharedCoreModels.Enums.ConnectionState.WaitForApprovel;
        private bool _showState;

        public Guid ConnectionId
        {
            get { return _connectionId; }
            set { SetProperty(ref _connectionId, value); }
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public SharedCoreModels.Enums.ConnectionState ConnectionState
        {
            get { return _connectionState; }
            set { SetProperty(ref _connectionState, value); }
        }

        public bool ShowState
        {
            get { return _showState; }
            set { SetProperty(ref _showState, value); }
        }
    }
}
