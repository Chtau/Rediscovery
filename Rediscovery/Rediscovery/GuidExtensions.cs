using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery
{
    public static class GuidExtensions
    {
        public static string ToNormalizedString(this Guid guid)
        {
            return guid.ToString().Replace("-", "").Trim();
        }
    }
}
