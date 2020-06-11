using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DALDesktopService.Models
{
    public class Device : DeviceMetadata
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
        public bool AllowAccess { get; set; }
        public string Role { get; set; }
    }
}
