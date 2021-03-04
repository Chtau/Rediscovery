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
using Java.Interop;
using Android.Webkit;

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    public class FeatureJSInterface : Java.Lang.Object
    {
        Context context;

        public FeatureJSInterface(Context context)
        {
            this.context = context;
        }

        [Export]
        [JavascriptInterface]
        public void ShowToast()
        {
            Toast.MakeText(context, "Hello from C#", ToastLength.Short).Show();
        }
    }
}