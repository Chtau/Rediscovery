using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
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
        Context _context;
        //readonly string vueJSContent = "";

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
            //vueJSContent = System.IO.File.ReadAllText(VueJS);
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
                string modelValue = JavascriptModel + "'Hello Test';";
                //{vueJSContent}
                Control.SetWebViewClient(new JavascriptWebViewClient($"javascript: {JavascriptFunction}{JavascriptSendCallbackFunction}{modelValue}"));
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                //string baseUrl = ((HtmlWebViewSource)((HybridWebView)Element).Source).BaseUrl;
                
                //Control.LoadDataWithBaseURL(baseUrl)
                //Control.LoadUrl($"file:///android_asset/Content/{((HybridWebView)Element).Uri}");
            }
        }
    }

    public class JavascriptWebViewClient : WebViewClient
    {
        string _javascript;

        public JavascriptWebViewClient(string javascript)
        {
            _javascript = javascript;
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.EvaluateJavascript(_javascript, null);
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
    }
}