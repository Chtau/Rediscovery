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

namespace Rediscovery.Client.App.MobileAndroid.Core
{
    public class CoreIO
    {
        public static string DefaultDirectory => System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
    }
}