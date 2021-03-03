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
    //@style/Rediscovery.NoActionBar
    //@style/Rediscovery
    [Activity(Label = "FeatureActivity", Theme = "@style/Rediscovery.NoActionBar", MainLauncher = false)]
    public class FeatureActivity : AppCompatActivity
    {
        public const string Key_DeviceId = "deviceid";

        public Guid DeviceId { get; private set; } = Guid.Empty;
        private bool isFabMenuOpen = false;
        private FloatingActionButton fab1;
        private FloatingActionButton fab2;
        private FloatingActionButton fab3;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            try
            {
                SetContentView(Resource.Layout.feature_detail);
                /*Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);*/
                var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
                fab1 = FindViewById<FloatingActionButton>(Resource.Id.fab1);
                fab2 = FindViewById<FloatingActionButton>(Resource.Id.fab2);
                fab3 = FindViewById<FloatingActionButton>(Resource.Id.fab3);
                fab.Click += (obj, args) =>
                {
                    OnToogleFabMenu();
                };

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
                    fab1.Animate().TranslationY(0);
                    fab2.Animate().TranslationY(0);
                    fab3.Animate().TranslationY(0);
                } else
                {
                    isFabMenuOpen = !isFabMenuOpen;
                    
                    fab1.Animate().TranslationY(Resources.GetDimension(Resource.Dimension.standard_55));
                    fab2.Animate().TranslationY(Resources.GetDimension(Resource.Dimension.standard_105));
                    fab3.Animate().TranslationY(Resources.GetDimension(Resource.Dimension.standard_155));
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Instance.Error(ex);
            }
        }
    }
}