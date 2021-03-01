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
    public class BaseViewOrderModel
    {
        public int ViewId { get; set; }
        public int OrderBy { get; set; }
        public bool IsFavorite { get; set; }
    }
}