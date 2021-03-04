using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
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
        private FloatingActionButton fabClose;
        private FloatingActionButton fabSetting;
        private FloatingActionButton fabSystem;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {
                SetContentView(Resource.Layout.feature_detail);
                
                var fabMenu = FindViewById<FloatingActionButton>(Resource.Id.fabMenu);
                fabMenu.Click += (_obj, _args) => OnToogleFabMenu();
                fabClose = FindViewById<FloatingActionButton>(Resource.Id.fabClose);
                fabClose.Click += (_obj, _args) => OnCloseAction();
                fabSetting = FindViewById<FloatingActionButton>(Resource.Id.fabSetting);
                fabSetting.Click += (_obj, _args) => OnSettingAction();
                fabSystem = FindViewById<FloatingActionButton>(Resource.Id.fabSystem);
                fabSystem.Click += (_obj, _args) => OnSystemAction();


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