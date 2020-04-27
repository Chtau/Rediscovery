using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopFeatures.FeaturePage.FeatureView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeatureView : ContentPage
    {
        private FeatureViewViewModel viewModel;

        private Features.Connection.IConnect connect => DependencyService.Get<Features.Connection.IConnect>() ?? new Features.Connection.Connect();
        private IFeatureUIService featureUIService => DependencyService.Get<IFeatureUIService>() ?? new FeatureUIService();

        public FeatureView(Guid desktopConfigId, Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature)
        {
            InitializeComponent();

            BindingContext = viewModel = new FeatureViewViewModel(desktopConfigId, connectionManifestFeature);
            viewModel.UIDataReady += ViewModel_UIDataReady;
        }

        private void ViewModel_UIDataReady(object sender, Tuple<Guid, string> e)
        {
            hybridWebView.SetFolderSource(e.Item2);
            hybridWebView.RegisterAction(async (data) =>
            {
                await DisplayAlert("Alert", "Hello " + data, "OK");
                Dispatcher.BeginInvokeOnMainThread(async () =>
                {
                    var result = await hybridWebView.EvaluateJavaScriptAsync("document.body.innerHTML");
                    System.Diagnostics.Debug.Print(result);
                });
            });
            
            hybridWebView.Navigated += async (obj, args) =>
            {
                //hybridWebView.SetModel(Newtonsoft.Json.JsonConvert.SerializeObject(DateTime.Now));
            };
            /*Task.Run(async () =>
            {
                await hybridWebView.SetModel(Newtonsoft.Json.JsonConvert.SerializeObject(DateTime.Now));
            });*/
            hybridWebView.SetModel(Newtonsoft.Json.JsonConvert.SerializeObject(DateTime.Now));
            Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(30000);
                    hybridWebView.SetModel(Newtonsoft.Json.JsonConvert.SerializeObject(DateTime.Now));
                } while (true);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}