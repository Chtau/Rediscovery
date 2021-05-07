using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomSheet;
using Rediscovery.Client.App.MobileAndroid.Features.Devices.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.Devices
{
    public class DeviceAddSheetFragment : Core.Controls.BaseBottomSheet<DeviceManageViewModel>
    {
        private Button btnTryConnect;
        private Button btnEditOk;
        private TextView ipAddress;
        private TextView portAddress;

        internal override int Layout => Resource.Layout.sheet_device_add;

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                ipAddress = view.FindViewById<TextView>(Resource.Id.addDeviceIP);
                portAddress = view.FindViewById<TextView>(Resource.Id.addDevicePort);

                btnTryConnect = view.FindViewById<Button>(Resource.Id.buttonFeatureEditTryConnect);
                btnTryConnect.Click += (_obj, _args) => OnTryConnect();
                btnEditOk = view.FindViewById<Button>(Resource.Id.buttonFeatureEditOk);
                btnEditOk.Click += (_obj, _args) => OnSave();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            base.OnViewCreated(view, savedInstanceState);
        }

        private void OnSave()
        {
            try
            {
                OnUpdateViewModel();
                OnInvokeAfterClose(ViewModel);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnTryConnect()
        {
            try
            {
                OnUpdateViewModel();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnUpdateViewModel()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ipAddress.Text))
                {
                    DeviceManageViewModel item = ViewModel;
                    if (item == null)
                    {
                        item = new DeviceManageViewModel();
                    }
                    item.IP = ipAddress.Text;
                    int port = -1;
                    if (!string.IsNullOrWhiteSpace(portAddress.Text))
                        if (int.TryParse(portAddress.Text, out port))
                            item.Port = port;
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}