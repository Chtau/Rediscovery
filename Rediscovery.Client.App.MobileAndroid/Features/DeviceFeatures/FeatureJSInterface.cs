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
using Java.Interop;
using Android.Webkit;

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    public class FeatureJSInterface : Java.Lang.Object
    {
        private readonly Context context;
        private Action<string> actionCallback;
        private Action domReadyCallback;
        private Action<string> loggerCallback;
        private Action<bool> loadCallback;

        public FeatureJSInterface(Context context)
        {
            this.context = context;
        }

        public void RegisterListener(Action<string> actionCallback, Action domReadyCallback, Action<string> loggerCallback, Action<bool> loadCallback)
        {
            this.actionCallback = actionCallback;
            this.domReadyCallback = domReadyCallback;
            this.loggerCallback = loggerCallback;
            this.loadCallback = loadCallback;
        }

        [Export]
        [JavascriptInterface]
        public void ShowToast()
        {
            Toast.MakeText(context, "Hello from C#", ToastLength.Short).Show();
        }

        [Export]
        [JavascriptInterface]
        public void InvokeAction(string data)
        {
            try
            {
                actionCallback?.Invoke(data);
            }
            catch (Exception ex)
            {
                loggerCallback?.Invoke(ex.ToString());
            }
        }

        [Export]
        [JavascriptInterface]
        public void InvokeDOMReady()
        {
            try
            {
                domReadyCallback?.Invoke();
            }
            catch (Exception ex)
            {
                loggerCallback?.Invoke(ex.ToString());
            }
        }

        [Export]
        [JavascriptInterface]
        public void InvokeLogger(string data)
        {
            try
            {
                loggerCallback?.Invoke(data);
            }
            catch (Exception ex)
            {
                loggerCallback?.Invoke(ex.ToString());
            }
        }

        [Export]
        [JavascriptInterface]
        public void InvokeLoadingState(bool load)
        {
            try
            {
                loadCallback?.Invoke(load);
            }
            catch (Exception ex)
            {
                loggerCallback?.Invoke(ex.ToString());
            }
        }
    }
}