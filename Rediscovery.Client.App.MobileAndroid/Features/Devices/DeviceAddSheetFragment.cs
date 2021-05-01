using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomSheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.Devices
{
    public class DeviceAddSheetFragment : BottomSheetDialogFragment
    {
        public event EventHandler AfterClose;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.device_bottom_sheet, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                
                var btnOk = view.FindViewById<Button>(Resource.Id.buttonFeatureEditOk);
                btnOk.Click += (_obj, _args) => OnClose(true);
                var btnCancel = view.FindViewById<Button>(Resource.Id.buttonFeatureEditCancel);
                btnCancel.Click += (_obj, _args) => OnClose(false);
                //OnLoad();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            base.OnViewCreated(view, savedInstanceState);
        }

        private void OnClose(bool save)
        {
            try
            {
                if (save)
                {
                    AfterClose?.Invoke(this, null);
                }
                else
                {
                    AfterClose?.Invoke(this, null);
                }
                Dismiss();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}