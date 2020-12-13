using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.Home
{
    public class DiscoveryFragment : AndroidX.Fragment.App.Fragment
    {
        public static DiscoveryFragment Create()
        {
            var args = new Bundle();
            //args.PutBoolean(ArgEdit, edit);
            var fragment = new DiscoveryFragment();
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_discovery, container, false);
        }
    }
}