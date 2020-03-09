using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public FeatureView()
        {
            InitializeComponent();

            hybridWebView.RegisterAction(data => DisplayAlert("Alert", "Hello " + data, "OK"));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(async () =>
            {
                try
                {
                    var client = await connect.GetHttpClientFeature();
                    //var uri = new Uri("features/ui/D5B218BC-8F36-4100-9262-71155265DAD7");

                    var response = await client.GetAsync("http://192.168.1.100:44341/features/ui/D5B218BC-8F36-4100-9262-71155265DAD7");
                    if (response.IsSuccessStatusCode)
                    {
                        var file = await response.Content.ReadAsStreamAsync();
                        //var content = await response.Content.ReadAsStringAsync();
                        //var Items = JsonConvert.DeserializeObject<List<TodoItem>>(content);
                    }
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.Print(ex.ToString());
                }
            });
        }
    }
}