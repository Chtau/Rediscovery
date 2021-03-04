using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    [Activity(Label = "FeatureActivity", Theme = "@style/Rediscovery.NoActionBar", MainLauncher = false)]
    public class FeatureActivity : AppCompatActivity
    {
        public const string Key_DeviceId = "deviceid";

        public Guid DeviceId { get; private set; } = Guid.Empty;
        private bool isFabMenuOpen = false;
        private FloatingActionButton fabMenu;
        private FloatingActionButton fabClose;
        private FloatingActionButton fabSetting;
        private FloatingActionButton fabSystem;
        private WebView webView;

        const string html = @"
<html>
  <body>
    <p>Demo calling C# from JavaScript</p>
<button type=""button"" onClick=""CSharp.ShowToast()"">Call C#</button>
  </body>
</html>";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {
                SetContentView(Resource.Layout.feature_detail);
                
                fabMenu = FindViewById<FloatingActionButton>(Resource.Id.fabMenu);
                fabMenu.Click += (_obj, _args) => OnToogleFabMenu();
                fabClose = FindViewById<FloatingActionButton>(Resource.Id.fabClose);
                fabClose.Click += (_obj, _args) => OnCloseAction();
                fabSetting = FindViewById<FloatingActionButton>(Resource.Id.fabSetting);
                fabSetting.Click += (_obj, _args) => OnSettingAction();
                fabSystem = FindViewById<FloatingActionButton>(Resource.Id.fabSystem);
                fabSystem.Click += (_obj, _args) => OnSystemAction();

                webView = FindViewById<WebView>(Resource.Id.webViewFeatureDetail);
                webView.Settings.JavaScriptEnabled = true;
                webView.SetWebViewClient(new AdvWebViewClient());
                webView.AddJavascriptInterface(new FeatureJSInterface(this), "CSharp");
                webView.LoadData(html, "text/html", null);

                var deviceIdString = Intent.Extras.GetString(Key_DeviceId);
                if (!string.IsNullOrWhiteSpace(deviceIdString))
                {
                    DeviceId = new Guid(deviceIdString);
                }
            } catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnToogleFabMenu()
        {
            try
            {
                var draw = fabMenu.Drawable;
                if (draw is AnimatedVectorDrawable animated)
                {
                    animated.Start();
                    if (isFabMenuOpen)
                        fabMenu.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.menu_transition, null));
                    else
                        fabMenu.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.menu_transition_back, null));
                }
                if (isFabMenuOpen)
                {
                    isFabMenuOpen = !isFabMenuOpen;
                    fabClose.Animate().TranslationY(0);
                    fabSetting.Animate().TranslationY(0);
                    fabSystem.Animate().TranslationY(0);
                } else
                {
                    isFabMenuOpen = !isFabMenuOpen;

                    fabClose.Animate().TranslationY(Resources.GetDimension(Resource.Dimension.standard_55));
                    fabSetting.Animate().TranslationY(Resources.GetDimension(Resource.Dimension.standard_105));
                    fabSystem.Animate().TranslationY(Resources.GetDimension(Resource.Dimension.standard_155));
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnCloseAction()
        {
            try
            {
                OnBackPressed();
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnSettingAction()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void OnSystemAction()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}