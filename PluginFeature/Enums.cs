using System;
using System.Collections.Generic;
using System.Text;

namespace PluginFeature
{
    public static class Enums
    {
        public enum PluginIntegration
        {
            Desktop = 0,
            Mobile = 1
        }

        [Flags]
        public enum ClientNativeResources
        {
            None = 0,
            OpenWithIntent = 1,
            FileTransfer = 2
        }
    }
}
