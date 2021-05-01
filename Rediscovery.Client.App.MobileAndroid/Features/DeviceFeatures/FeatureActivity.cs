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
using System.Threading.Tasks;

namespace Rediscovery.Client.App.MobileAndroid.Features.DeviceFeatures
{
    //Theme = "@style/Rediscovery.NoActionBar",
    [Activity(Label = "FeatureActivity",  MainLauncher = false)]
    public class FeatureActivity : AppCompatActivity
    {
        public const string Key_DeviceId = "deviceid";
        public const string Key_FeatureId = "featureid";

        public Guid DeviceId { get; private set; } = Guid.Empty;
        public Guid FeatureId { get; private set; } = Guid.Empty;
        private bool isFabMenuOpen = false;
        private FloatingActionButton fabMenu;
        private FloatingActionButton fabClose;
        private FloatingActionButton fabSetting;
        private FloatingActionButton fabSystem;
        private WebView webView;

        const string html = @"
<html style='height:100%'>
<head>
<style>
.center {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 200px;
  margin-top: auto;
  margin-bottom: auto;
  border: 3px solid green; 
}
</style>
</head>
  <body style='height:100%;margin-top: 150px;'>
<div class='center'>
<p>Demo calling C# from JavaScript</p>
<button type=""button"" onClick=""Feature.ShowToast()"">Call C#</button>
</div>
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
                webView.SetWebViewClient(new AdvWebViewClient(OnGetDefaultJS(), LoggerCallback));
                var jsInterface = new FeatureJSInterface(this);
                jsInterface.RegisterListener(ActionCallback, DOMReadyCallback, LoggerCallback, LoadCallback);
                webView.AddJavascriptInterface(jsInterface, "Feature");
                webView.LoadData(html, "text/html", null);
                var deviceIdString = Intent.Extras.GetString(Key_DeviceId);
                if (!string.IsNullOrWhiteSpace(deviceIdString))
                {
                    DeviceId = new Guid(deviceIdString);
                }
                var featureIdString = Intent.Extras.GetString(Key_FeatureId);
                if (!string.IsNullOrWhiteSpace(featureIdString))
                {
                    FeatureId = new Guid(featureIdString);
                }
            } catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void ActionCallback(string data)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void DOMReadyCallback()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void LoggerCallback(string data)
        {
            try
            {
                Core.Logger.Instance.Error(new FeatureException(data, true));
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        private void LoadCallback(bool load)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }

        public override void OnBackPressed()
        {
            //OnCreateScreenThumbnail();
            base.OnBackPressed();
        }

        protected override void OnDestroy()
        {
            OnCreateScreenThumbnail();
            base.OnDestroy();
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

        private void OnCreateScreenThumbnail()
        {
            try
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageWrite>();
                    var status = await Xamarin.Essentials.Permissions.CheckStatusAsync<Xamarin.Essentials.Permissions.StorageWrite>();
                    if (status == Xamarin.Essentials.PermissionStatus.Granted)
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                var screen = webView.TakeScreenshot();
                                var thumbnail = Android.Media.ThumbnailUtils.ExtractThumbnail(screen, 350, 350);
                                var file = System.IO.Path.Combine(Core.CoreIO.Instance.DeviceFeatureThumbnailDirectory(DeviceId), $"{FeatureId.ToSafeString()}.png");
                                var stream = new FileStream(file, FileMode.Create);
                                thumbnail.Compress(Bitmap.CompressFormat.Png, 75, stream);
                                stream.Close();
                                // TODO: update feature UI from the device if it is still open
                            }
                            catch (Exception ex)
                            {
                                Core.Logger.Instance.Error(ex);
                            }
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}