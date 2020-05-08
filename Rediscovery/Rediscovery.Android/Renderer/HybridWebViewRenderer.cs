using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using Rediscovery.Controls;
using Rediscovery.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace Rediscovery.Droid.Renderer
{
    public class HybridWebViewRenderer : WebViewRenderer
    {
        //const string VueJS = "file:///android_asset/Content/vue.dev.js";
        const string JavascriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);};";
        const string JavascriptSendCallbackFunction = "function featureSend(data){jsBridge.invokeAction(data);};";
        const string JavascriptModel = "var model = ";
        const string JavascriptExchangeFile = "Content/exchange.js";
        Context _context;
        readonly string exchangeJSContent = "";

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
            AssetManager assets = context.Assets;
            using (StreamReader sr = new StreamReader(assets.Open(JavascriptExchangeFile)))
            {
                exchangeJSContent = sr.ReadToEnd();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                ((HybridWebView)Element).Cleanup();
            }
            if (e.NewElement != null)
            {
                Control.SetWebViewClient(new JavascriptWebViewClient($"javascript: {exchangeJSContent}", (error) =>
                {
                    ((HybridWebView)Element).InvokeError(error);
                }));
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
            }
        }
    }

    public class JavascriptWebViewClient : WebViewClient
    {
        private readonly string _javascript;
        private readonly Action<string> _error;

        public JavascriptWebViewClient(string javascript, Action<string> error)
        {
            _javascript = javascript;
            _error = error;
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.EvaluateJavascript(_javascript, null);
        }

        public override void OnReceivedError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error)
        {
            base.OnReceivedError(view, request, error);
            _error.Invoke($"Code:{Enum.GetName(typeof(ClientError), error.ErrorCode)} Description:{error.Description}");
        }
    }

    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<HybridWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(HybridWebViewRenderer hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<HybridWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            HybridWebViewRenderer hybridRenderer;

            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                ((HybridWebView)hybridRenderer.Element).InvokeAction(data);
            }
        }

        [JavascriptInterface]
        [Export("invokeDOMReady")]
        public void InvokeDOMReady()
        {
            HybridWebViewRenderer hybridRenderer;

            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                ((HybridWebView)hybridRenderer.Element).InvokeDOMReady();
            }
        }
    }
}