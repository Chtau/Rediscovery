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

namespace Rediscovery.Client.App.MobileAndroid.Features.Devices.ViewModels
{
    public class DeviceAddViewModel
    {
        public string IP { get; set; }
        public int Port { get; set; } = -1;
    }
}