using PluginFeature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rediscovery.Features.DesktopFeatures
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesktopFeaturesPage : ContentPage
    {
        DesktopFeaturesViewModel viewModel;

        public DesktopFeaturesPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new DesktopFeaturesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            Features.Connection.Models.ConnectionManifestFeature item = args.SelectedItem as Features.Connection.Models.ConnectionManifestFeature;
            if (item != null)
            {
                await Navigation.PushModalAsync(new NavigationPage(new FeaturePage.FeatureView.FeatureView(item.ConfigurationId, item)));
                FeatureControl.SelectedItem = null;
            }
        }

        private void Filter_Clicked(object sender, EventArgs e)
        {

        }
    }
}