using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    public class AdvWebViewClient : WebViewClient
    {
        private readonly string _javascript;
        private readonly Action<string> _errorCallback;

        public AdvWebViewClient(string javascript, Action<string> errorCallback)
        {
            _javascript = javascript;
            _errorCallback = errorCallback;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            if (!string.IsNullOrWhiteSpace(_javascript))
                view.EvaluateJavascript(_javascript, null);
        }

        public override void OnReceivedError(WebView view, IWebResourceRequest request, WebResourceError error)
        {
            base.OnReceivedError(view, request, error);
            _errorCallback.Invoke($"Code:\"{Enum.GetName(typeof(ClientError), error.ErrorCode)}\" Description:\"{error.Description}\"");
        }
    }
}