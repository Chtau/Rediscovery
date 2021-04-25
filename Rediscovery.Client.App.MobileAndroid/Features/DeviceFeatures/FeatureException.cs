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

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    public class FeatureException : Exception
    {
        public bool InWebView { get; set; }

        public FeatureException(string message, bool inWebView = false) : base(message)
        {
            InWebView = inWebView;
        }
    }
}