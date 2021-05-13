using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using Rediscovery.Client.App.MobileAndroid.Core.Controls;
using Rediscovery.Client.App.MobileAndroid.Features.Devices.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.Devices
{
    public class HostManagmentFragment : AndroidX.Fragment.App.Fragment, IBottomSheetSupport<HostEditSheetFragment, ViewModels.HostManageViewModel>
    {
        private FloatingActionButton addDeviceButton;
        private RecyclerView recyclerView;
        private HostManagmentAdapter deviceManagmentAdapter;
        private IMenu managmentMenu;

        public event EventHandler<ViewModels.HostManageViewModel> DeviceConnectRequested;
        public event EventHandler<BottomSheetEventArgs<HostEditSheetFragment, HostManageViewModel>> OpenSheet;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
            try
            {
                if (savedInstanceState != null)
                {
                    /*var deviceIdString = savedInstanceState.GetString(Key_Feature_DeviceId);
                    if (!string.IsNullOrWhiteSpace(deviceIdString))
                    {
                        var deviceId = new Guid(deviceIdString);
                        OnLoad(deviceId);
                    }*/
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.host_managment_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            addDeviceButton = view.FindViewById<FloatingActionButton>(Resource.Id.addDeviceButton);
            addDeviceButton.Click += AddDeviceButton_Click;
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.deviceManagment);
            var layoutManager = new LinearLayoutManager(Context);
            recyclerView.SetLayoutManager(layoutManager);
            OnSetNewDeviceManagment();
            base.OnViewCreated(view, savedInstanceState);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            managmentMenu = menu;
            base.OnCreateOptionsMenu(menu, inflater);
            try
            {
                menu.Clear();
                //inflater.Inflate(Resource.Menu.menu_features, menu);
                //OnUpdateMenuState(menu);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            //outState.PutString(Key_Feature_DeviceId, Device?.DeviceId.ToString());
            base.OnSaveInstanceState(outState);
        }

        public override void OnResume()
        {
            deviceManagmentAdapter.NotifyDataSetChanged();
            base.OnResume();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                int id = item.ItemId;
                switch (id)
                {
                    /*case Resource.Id.action_features_device_add_favorite:
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
                    case Resource.Id.action_features_device_switch_show_local_features:
                    case Resource.Id.action_features_device_switch_show_remote_features:
                        // switch between features available on the connected device and feature provided to the device => show remote
                        showLocalFeatures = id == Resource.Id.action_features_device_switch_show_local_features;
                        OnSetNewFeatures(showLocalFeatures);
                        OnUpdateMenuState(deviceMenu);
                        return true;*/
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            return base.OnOptionsItemSelected(item);
        }

        private void OnSetNewDeviceManagment()
        {
            try
            {
                if (deviceManagmentAdapter != null)
                {
                    //deviceManagmentAdapter.LayoutClick -= DeviceFeaturesAdapter_LayoutClick;
                    //deviceManagmentAdapter.ButtonActionClick -= DeviceFeaturesAdapter_ButtonActionClick;
                }
                deviceManagmentAdapter = new HostManagmentAdapter(Activity);
                deviceManagmentAdapter.ConnectModel += DeviceManagmentAdapter_ConnectModel;
                deviceManagmentAdapter.EditModel += DeviceManagmentAdapter_EditModel;
                recyclerView.SwapAdapter(deviceManagmentAdapter, true);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void AddDeviceButton_Click(object sender, EventArgs e)
        {
            try
            {
                OnOpenSheetRequest(new HostManageViewModel(null, null, -1));
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void DeviceManagmentAdapter_EditModel(object sender, ViewModels.HostManageViewModel e)
        {
            try
            {
                OnOpenSheetRequest(e);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnOpenSheetRequest(HostManageViewModel viewModel)
        {
            OpenSheet.Invoke(this, new BottomSheetEventArgs<HostEditSheetFragment, HostManageViewModel>(new HostEditSheetFragment(), viewModel));
        }

        private void DeviceManagmentAdapter_ConnectModel(object sender, ViewModels.HostManageViewModel e)
        {
            try
            {
                DeviceConnectRequested?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public void AfterCloseSheet(HostManageViewModel viewModel)
        {
            try
            {
                // TODO: implement refresh after a change from this callback
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}