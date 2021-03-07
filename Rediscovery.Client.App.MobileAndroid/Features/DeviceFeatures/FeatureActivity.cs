using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.FloatingActionButton;
using Java.IO;
using System;
using System.Collections.Generic;
using System.IO;
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
<button type=""button"" onClick=""Feature.ShowToast()"">Call C#</button>
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
                webView.SetWebViewClient(new AdvWebViewClient(OnGetDefaultJS(), (error) =>
                {
                    Core.Logger.Instance.Error(new Exception(error));
                }));
                webView.AddJavascriptInterface(new FeatureJSInterface(this), "Feature");
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

        private string OnGetDefaultJS()
        {
            string defaultJS = null;
            try
            {
                using (StreamReader sr = new StreamReader(Assets.Open("Content/feature.js")))
                {
                    defaultJS = sr?.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
            return defaultJS;
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

        private async void OnSystemAction()
        {
            try
            {
                await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageWrite>();
                var status = await Xamarin.Essentials.Permissions.CheckStatusAsync<Xamarin.Essentials.Permissions.StorageWrite>();
                if (status == Xamarin.Essentials.PermissionStatus.Granted)
                {
                    var screen = OnTakeScreenshot(webView);
                    var pubDocs = Core.CoreIO.Instance.DefaultDirectory; //Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
                    var file = System.IO.Path.Combine(pubDocs, $"{DateTime.Now:yyyyMMddHHmmss}.png");
                    var stream = new FileStream(file, FileMode.Create);
                    screen.Compress(Bitmap.CompressFormat.Png, 85, stream);
                    stream.Close();
                    Core.CoreIO.Instance.AddPublicFile(file);
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private Bitmap OnTakeScreenshot(View view)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            view.Draw(canvas);
            return bitmap;
        }
    }
}