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

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    public class FeaturesDashboardFragment : AndroidX.Fragment.App.Fragment
    {
        private DeviceFeaturesAdapter deviceFeaturesAdapter;

        public static FeaturesDashboardFragment Create()
        {
            var args = new Bundle();
            //args.PutBoolean(ArgEdit, edit);
            var fragment = new FeaturesDashboardFragment();
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
            return inflater.Inflate(Resource.Layout.fragment_dashboard_features, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            OnSetUpFeatures(view.FindViewById<GridView>(Resource.Id.devicefeatures));
            base.OnViewCreated(view, savedInstanceState);
        }

        public override void OnResume()
        {
            deviceFeaturesAdapter.NotifyDataSetChanged();
            base.OnResume();
        }

        private void OnSetUpFeatures(GridView featuresGridView)
        {
            featuresGridView.ItemClick += (obj, args) =>
            {
                // TODO: open feature
            };
            deviceFeaturesAdapter = new DeviceFeaturesAdapter(Activity);
            featuresGridView.Adapter = deviceFeaturesAdapter;
        }
    }
}