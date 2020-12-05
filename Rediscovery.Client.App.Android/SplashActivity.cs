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
using System.Threading.Tasks;

namespace Rediscovery.Client.App.Android
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", Theme = "@style/AppThemeRediscovery.Splash", MainLauncher = true, NoHistory = true)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"*/*")]
    public class SplashActivity : Activity
    {
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => OnStartup());
            startupWork.Start();
        }

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