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
        private Features.Connection.IConnect connect => DependencyService.Get<Features.Connection.IConnect>() ?? new Features.Connection.Connect();
        private IFeatureUIService featureUIService => DependencyService.Get<IFeatureUIService>() ?? new FeatureUIService();

        public FeatureView()
        {
            InitializeComponent();

            string baseUrl = featureUIService.UIDirectory(new Guid("D5B218BC-8F36-4100-9262-71155265DAD7"));
            var source = new HtmlWebViewSource();
            source.BaseUrl = "file://" + baseUrl + "/";
            source.Html = System.IO.File.ReadAllText(System.IO.Path.Combine(baseUrl, "Index.html"));
            hybridWebView.Source = source;
            //hybridWebView.Uri = "Index.html";
            hybridWebView.RegisterAction(data => DisplayAlert("Alert", "Hello " + data, "OK"));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}