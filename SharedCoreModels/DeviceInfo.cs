using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCoreModels
{
    public class DeviceInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool AllowAccess { get; set; }
    }
}
