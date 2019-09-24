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
    }
}
