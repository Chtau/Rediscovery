using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Manager.Console
{
    public static class StringExtensions
    {
        public static string PutifyStringArray(this string[] array)
        {
            if (array != null)
                return string.Join(" / ", array);
            return "";
        }
    }
}
