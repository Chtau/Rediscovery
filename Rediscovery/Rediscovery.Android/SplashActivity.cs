using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Rediscovery.Droid
{
    [Activity(Label = "Rediscovery", Icon = "@mipmap/icon", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"*/*")]
    public class SplashActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => OnStartup());
            startupWork.Start();
        }

        // Prevent the back button from canceling the startup process
        public override void OnBackPressed() { }

        private void OnStartup()
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));
            if (Intent.Action == Intent.ActionSend)
            {
                intent.PutExtras(Intent);
                intent.SetAction(Intent.ActionSend);
                intent.ClipData = Intent.ClipData;
                intent.SetType(Intent.Type);
            }
            StartActivity(intent);
        }
    }
}