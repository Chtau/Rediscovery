using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationFeatureProvider
{
    public static class FeatureActiveDevices
    {
        public static HashSet<string> Devices = new HashSet<string>();

        internal static void AddDevice(string sid)
        {
            if (!Devices.Contains(sid))
                Devices.Add(sid);
        }

        internal static void RemoveDevice(string sid)
        {
            if (Devices.Contains(sid))
                Devices.Remove(sid);
        }
    }
}
