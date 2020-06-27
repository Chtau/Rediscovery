using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Rediscovery.Droid
{
    [Activity(Label = "Rediscovery", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"application/pdf")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var app = new App();
            LoadApplication(app);

            if (Intent.Action == Intent.ActionSend)
            {
                var pdf = Intent.ClipData.GetItemAt(0);

                var uriFromExtras = Intent.GetParcelableExtra(Intent.ExtraStream) as Android.Net.Uri;
                var subject = Intent.GetStringExtra(Intent.ExtraSubject);

                var pdfStream = ContentResolver.OpenInputStream(pdf.Uri);

                var memOfPdf = new System.IO.MemoryStream();
                pdfStream.CopyTo(memOfPdf);

                var docsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var filePath = System.IO.Path.Combine(docsPath, "temp.pdf");

                System.IO.File.WriteAllBytes(filePath, memOfPdf.ToArray());

                app.PDFIntent(filePath);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
    }
}