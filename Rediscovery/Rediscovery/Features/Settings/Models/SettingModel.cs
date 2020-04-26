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

        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string DeviceIdentifier
        {
            get { return _deviceIdentifier; }
            set { SetProperty(ref _deviceIdentifier, value); }
        }

        public SettingModel()
        {
            Id = Guid.Empty;
        }
    }
}
