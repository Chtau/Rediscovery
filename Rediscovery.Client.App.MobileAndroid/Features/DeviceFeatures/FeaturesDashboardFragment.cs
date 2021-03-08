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
using System.Threading.Tasks;

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    public class FeaturesDashboardFragment : AndroidX.Fragment.App.Fragment
    {
        private const string Key_Feature_DeviceId = "featuredeviceid";

        private GridView featureGridView;
        private DeviceFeaturesAdapter deviceFeaturesAdapter;
        private IMenu deviceMenu;

        public event EventHandler DeviceFavoriteChanged;
        public event EventHandler<ViewModels.FeatureViewModel> FeatureSheetRequested;

        public Features.Models.Device Device { get; private set; }

        public static FeaturesDashboardFragment Create(Guid deviceId)
        {
            var args = new Bundle();
            //args.PutBoolean(ArgEdit, edit);
            var fragment = new FeaturesDashboardFragment(deviceId);
            fragment.Arguments = args;
            return fragment;
        }

        public FeaturesDashboardFragment()
        {

        }

        public FeaturesDashboardFragment(Guid deviceId) : this()
        {
            OnLoad(deviceId);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
            try
            {
                var deviceIdString = savedInstanceState.GetString(Key_Feature_DeviceId);
                if (!string.IsNullOrWhiteSpace(deviceIdString))
                {
                    var deviceId = new Guid(deviceIdString);
                    OnLoad(deviceId);
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_dashboard_features, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            featureGridView = view.FindViewById<GridView>(Resource.Id.devicefeatures);
            OnSetUpFeatures(featureGridView);
            base.OnViewCreated(view, savedInstanceState);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            deviceMenu = menu;
            base.OnCreateOptionsMenu(menu, inflater);
            try
            {
                menu.Clear();
                inflater.Inflate(Resource.Menu.menu_features, menu);
                OnUpdateMenuState(menu);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(Key_Feature_DeviceId, Device?.DeviceId.ToString());
            base.OnSaveInstanceState(outState);
        }

        private void OnLoad(Guid deviceId)
        {
            try
            {
                Device = Manager.DeviceManager.Instance.Get(deviceId);
                if (Device == null)
                {
                    // new entry
                    Device = new Models.Device();
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        /// <summary>
        /// toggle menu items for add/remove favorite and connect/disconnect
        /// </summary>
        /// <param name="menu"></param>
        private void OnUpdateMenuState(IMenu menu)
        {
            try
            {
                if (Device.IsFavorite)
                {
                    OnMenuToggle(menu, false, Resource.Id.action_features_device_add_favorite, Resource.Id.action_features_device_remove_favorite);
                }
                else
                {
                    OnMenuToggle(menu, true, Resource.Id.action_features_device_add_favorite, Resource.Id.action_features_device_remove_favorite);
                }
                if (Device.IsConnected)
                {
                    OnMenuToggle(menu, false, Resource.Id.action_features_device_connect, Resource.Id.action_features_device_disconnect);
                }
                else
                {
                    OnMenuToggle(menu, true, Resource.Id.action_features_device_connect, Resource.Id.action_features_device_disconnect);
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public void UpdateFeatureGrid()
        {
            try
            {
                if (deviceFeaturesAdapter != null)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                        {
                            deviceFeaturesAdapter.LayoutClick -= DeviceFeaturesAdapter_LayoutClick;
                            deviceFeaturesAdapter.ButtonActionClick -= DeviceFeaturesAdapter_ButtonActionClick;
                            deviceFeaturesAdapter = new DeviceFeaturesAdapter(Activity, Device, null, null);
                            deviceFeaturesAdapter.LayoutClick += DeviceFeaturesAdapter_LayoutClick;
                            deviceFeaturesAdapter.ButtonActionClick += DeviceFeaturesAdapter_ButtonActionClick;
                            featureGridView.Adapter = deviceFeaturesAdapter;
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnMenuToggle(IMenu menu, bool activeFirst, int first, int second)
        {
            var menuItemOn = menu.FindItem(first);
            if (menuItemOn != null)
                menuItemOn.SetVisible(activeFirst);
            var menuItemOff = menu.FindItem(second);
            if (menuItemOff != null)
                menuItemOff.SetVisible(!activeFirst);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                int id = item.ItemId;
                switch (id)
                {
                    case Resource.Id.action_features_device_add_favorite:
                        OnChangeFavorite(true);
                        return true;
                    case Resource.Id.action_features_device_remove_favorite:
                        OnChangeFavorite(false);
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
            } catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnResume()
        {
            deviceFeaturesAdapter.NotifyDataSetChanged();
            base.OnResume();
        }

        private void OnSetUpFeatures(GridView gridView)
        {
            try
            {
                deviceFeaturesAdapter = new DeviceFeaturesAdapter(Activity, Device, null, null);
                deviceFeaturesAdapter.LayoutClick += DeviceFeaturesAdapter_LayoutClick;
                deviceFeaturesAdapter.ButtonActionClick += DeviceFeaturesAdapter_ButtonActionClick;
                gridView.Adapter = deviceFeaturesAdapter;
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void DeviceFeaturesAdapter_ButtonActionClick(object sender, ViewModels.FeatureViewModel e)
        {
            OnFeatureViewModelButtonAction(e);
        }

        private void DeviceFeaturesAdapter_LayoutClick(object sender, ViewModels.FeatureViewModel e)
        {
            OnFeatureViewModelLayout(e);
        }

        private void OnChangeFavorite(bool isFavorite)
        {
            try
            {
                Device.IsFavorite = isFavorite;
                Manager.DeviceManager.Instance.Save(Device);
                OnUpdateMenuState(deviceMenu);
                DeviceFavoriteChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private Guid lastFeatureViewButtonActionId = Guid.Empty;
        private void OnFeatureViewModelButtonAction(ViewModels.FeatureViewModel featureViewModel)
        {
            try
            {
                if (lastFeatureViewButtonActionId != featureViewModel.Feature.FeatureId)
                {
                    lastFeatureViewButtonActionId = featureViewModel.Feature.FeatureId;
                    Core.Logger.Instance.Debug($"Feature Action tab ID:{featureViewModel.Feature.FeatureId}");
                    FeatureSheetRequested?.Invoke(this, featureViewModel);
                    Task.Run(async () => 
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        lastFeatureViewButtonActionId = Guid.Empty;
                    });
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnFeatureViewModelLayout(ViewModels.FeatureViewModel featureViewModel)
        {
            try
            {
                Core.Logger.Instance.Debug($"Feature Action tab ID:{featureViewModel.Feature.FeatureId}");

                var intent = new Intent(Application.Context, typeof(FeatureActivity));
                intent.PutExtra(FeatureActivity.Key_DeviceId, Device.DeviceId.ToString());
                intent.PutExtra(FeatureActivity.Key_FeatureId, featureViewModel.Feature.FeatureId.ToString());
                MainActivity.Instance.StartActivityForResult(intent, MainActivity.Intent_Feature_Id);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}