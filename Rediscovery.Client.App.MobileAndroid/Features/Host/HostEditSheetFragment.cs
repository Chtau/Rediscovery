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
    public class HostEditSheetFragment : Core.Controls.BaseBottomSheet<HostManageViewModel>
    {
        private Button btnTryConnect;
        private Button btnEditSave;
        private TextView friendlyName;
        private TextView ipAddress;
        private TextView portAddress;

        internal override int Layout => Resource.Layout.host_managment_edit_buttom_sheet;

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                friendlyName = view.FindViewById<TextView>(Resource.Id.hostEditFriendlyName);
                ipAddress = view.FindViewById<TextView>(Resource.Id.hostEditIP);
                portAddress = view.FindViewById<TextView>(Resource.Id.hostEditPort);

                btnTryConnect = view.FindViewById<Button>(Resource.Id.btnHostEditTryConnect);
                btnTryConnect.Click += (_obj, _args) => OnTryConnect();
                btnEditSave = view.FindViewById<Button>(Resource.Id.btnHostEditSave);
                btnEditSave.Click += (_obj, _args) => OnSave();

                friendlyName.Text = ViewModel.FriendlyName;
                ipAddress.Text = ViewModel.IP;
                if (ViewModel.Port > 0)
                    portAddress.Text = ViewModel.Port.ToString();
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
                    HostManageViewModel item = ViewModel;
                    if (item == null)
                    {
                        item = new HostManageViewModel(null, null, -1);
                    }
                    item.FriendlyName = friendlyName.Text;
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