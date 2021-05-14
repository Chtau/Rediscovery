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

namespace Rediscovery.Client.App.MobileAndroid.Features.Models
{
    public class Host
    {
        public Guid HostId { get; set; }
        public string FriendlyName { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public bool AutoConnect { get; set; }
        public List<Device> Devices { get; set; }
    }
}