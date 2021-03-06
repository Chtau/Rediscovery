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

        public Features.Models.Device Device { get; }

        public static FeaturesDashboardFragment Create(Guid deviceId)
        {
            var args = new Bundle();
            //args.PutBoolean(ArgEdit, edit);
            var fragment = new FeaturesDashboardFragment(deviceId);
            fragment.Arguments = args;
            return fragment;
        }

        public FeaturesDashboardFragment(Guid deviceId)
        {
            try
            {
                Device = Core.Database.Instance.Get<Features.Models.Device>(x => x.DeviceId == deviceId).FirstOrDefault();
                if (Device == null)
                {
                    // new entry
                    Device = new Models.Device();
                }
            } catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
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

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            menu.Clear();
            inflater.Inflate(Resource.Menu.menu_features, menu);
            // TODO: toggle menu items for add/remove favorite and connect/disconnect
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            switch (id)
            {
                case Resource.Id.action_features_device_add_favorite:
                    return true;
                case Resource.Id.action_features_device_remove_favorite:
                    return true;
                case Resource.Id.action_features_device_connect:
                    return true;
                case Resource.Id.action_features_device_disconnect:
                    return true;
                case Resource.Id.action_features_device_detail:
                    return true;
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnResume()
        {
            deviceFeaturesAdapter.NotifyDataSetChanged();
            base.OnResume();
        }

        private void OnSetUpFeatures(GridView featuresGridView)
        {
            try
            {
                featuresGridView.ItemClick += (obj, args) =>
                {
                    try
                    {
                        var intent = new Intent(Application.Context, typeof(FeatureActivity));
                        intent.PutExtra(FeatureActivity.Key_DeviceId, Device.DeviceId.ToString());
                        StartActivity(intent);
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Instance.Error(ex);
                    }
                };
                
                deviceFeaturesAdapter = new DeviceFeaturesAdapter(Activity, Device);
                featuresGridView.Adapter = deviceFeaturesAdapter;
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}