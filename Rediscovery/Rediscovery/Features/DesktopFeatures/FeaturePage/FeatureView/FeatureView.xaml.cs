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

        public FeatureView(Features.Connection.Models.ConnectionManifestFeature connectionManifestFeature)
        {
            InitializeComponent();

            BindingContext = viewModel = new FeatureViewViewModel(connectionManifestFeature);
            viewModel.UIDataReady += ViewModel_UIDataReady;
        }

        private void ViewModel_UIDataReady(object sender, Tuple<Guid, string> e)
        {
            hybridWebView.SetFolderSource(e.Item2);
            hybridWebView.RegisterAction(data => DisplayAlert("Alert", "Hello " + data, "OK"));
            Task.Run(async () =>
            {
                await hybridWebView.SetModel(Newtonsoft.Json.JsonConvert.SerializeObject(DateTime.Now));
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}