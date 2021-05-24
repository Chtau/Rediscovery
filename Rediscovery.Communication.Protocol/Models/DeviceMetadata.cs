using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Models
{
    public class DeviceMetadata
    {
        public enum IdiomType
        {
            Undefined = 0,
            Desktop = 1,
            Phone = 2,
            Tablet = 3
        }

        public string OS { get; set; } = Environment.OSVersion?.ToString();
        public IdiomType Idiom { get; set; } = IdiomType.Undefined;
        public string User { get; set; } = Environment.UserName;
        public string Machine { get; set; } = Environment.MachineName;
        public bool Is64Bit { get; set; } = Environment.Is64BitOperatingSystem;
        public int Processor { get; set; } = Environment.ProcessorCount;
        public long PhysicalMemory { get; set; } = Environment.WorkingSet;
    }
}
