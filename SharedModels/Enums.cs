using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase
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
