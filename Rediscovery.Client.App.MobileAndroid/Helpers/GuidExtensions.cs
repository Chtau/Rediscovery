using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid
{
    public static class GuidExtensions
    {
        public static string ToSafeString(this Guid value)
        {
            return value.ToString().Replace("-", "").Trim().ToLower();
        }
    }
}