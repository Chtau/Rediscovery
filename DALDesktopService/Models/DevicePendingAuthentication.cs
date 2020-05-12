using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DALDesktopService.Models
{
    public class DevicePendingAuthentication
    {
        private string deviceIdentifier { get; set; }

        [PrimaryKey]
        public Guid Id { get; set; }
        public string DeviceName { get; set; }
        public string DeviceIdentifier
        {
            get { return deviceIdentifier; }
            set
            {
                deviceIdentifier = value?.ToLower();
            }
        }
        public DateTime RequestTime { get; set; }
    }
}
