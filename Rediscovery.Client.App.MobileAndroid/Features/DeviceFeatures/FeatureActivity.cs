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
    [Activity(Label = "FeatureActivity")]
    public class FeatureActivity : Activity
    {
        public const string Key_DeviceId = "deviceid";

        public Guid DeviceId { get; private set; } = Guid.Empty;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {
                var deviceIdString = Intent.Extras.GetString(Key_DeviceId);
                if (!string.IsNullOrWhiteSpace(deviceIdString))
                {
                    DeviceId = new Guid(deviceIdString);
                }
            } catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}