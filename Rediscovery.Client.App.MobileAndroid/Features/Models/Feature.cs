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
    public class Feature : BaseViewOrderModel
    {
        public Guid FeatureId { get; set; }
        public string Name { get; set; }
        public int DisplayTheme { get; set; } = 1;
    }
}