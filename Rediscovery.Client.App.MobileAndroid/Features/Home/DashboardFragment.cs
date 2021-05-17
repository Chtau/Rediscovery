using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Snackbar;
using Rediscovery.Communication.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Client.App.MobileAndroid.Features.Home
{
    public class DashboardFragment : AndroidX.Fragment.App.Fragment
    {
        private Button btnPlayPauseDevice;
        private Button btnAddDevice;
        private Button btnTestSocketListen;
        private Button btnTestSocketSend;

        private bool isAutoDiscoverDevices = false;

        private RediscoveryProtocol protocol = new RediscoveryProtocol();

        public event EventHandler AddDeviceRequest;

        public static DashboardFragment Create()
        {
            var args = new Bundle();
            //args.PutBoolean(ArgEdit, edit);
            var fragment = new DashboardFragment();
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
            return inflater.Inflate(Resource.Layout.fragment_dashboard, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            try
            {
                btnTestSocketListen = view.FindViewById<Button>(Resource.Id.btnSocketListenTest);
                btnTestSocketListen.Click += BtnTestSocketListen_Click;
                btnTestSocketSend = view.FindViewById<Button>(Resource.Id.btnSocketSendTest);
                btnTestSocketSend.Click += BtnTestSocketSend_Click;
                /*btnPlayPauseDevice = view.FindViewById<Button>(Resource.Id.buttonDevicesPlayPause);
                btnAddDevice = view.FindViewById<Button>(Resource.Id.buttonDeviceAdd);
                btnAddDevice.Click += BtnAddDevice_Click;
                btnPlayPauseDevice.Click += BtnPlayPauseDevice_Click;*/

                OnToggleAutoDiscoverDevices(false);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            base.OnViewCreated(view, savedInstanceState);
        }

        private void BtnTestSocketSend_Click(object sender, EventArgs e)
        {
            try
            {
                protocol.Send("192.168.1.103");
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void BtnTestSocketListen_Click(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() =>
                {
                    protocol.Listen((transfer) =>
                    {
                        var result = System.Text.ASCIIEncoding.ASCII.GetString(transfer.Content);
                        System.Diagnostics.Trace.TraceInformation($"Socket received:{result}");

                        Snackbar.Make(View, $"Socket received:{result}", Snackbar.LengthLong).Show();
                    });
                });
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void BtnPlayPauseDevice_Click(object sender, EventArgs e)
        {
            try
            {
                OnToggleAutoDiscoverDevices(!isAutoDiscoverDevices);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnToggleAutoDiscoverDevices(bool newState)
        {
            try
            {
                if (newState)
                {
                    btnPlayPauseDevice.SetBackgroundResource(Resource.Drawable.selector_button_pause);
                }
                else
                {
                    btnPlayPauseDevice.SetBackgroundResource(Resource.Drawable.selector_button_play);
                }
                isAutoDiscoverDevices = newState;
                // TODO: trigger auto discover change in device manager
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void BtnAddDevice_Click(object sender, EventArgs e)
        {
            try
            {
                AddDeviceRequest?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}