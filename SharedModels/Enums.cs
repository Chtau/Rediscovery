using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Shared.Base
{
    public static class Enums
    {
        [Flags]
        public enum ClientNativeResources
        {
            None = 0,
            OpenWithIntent = 1
        }
    }
}
